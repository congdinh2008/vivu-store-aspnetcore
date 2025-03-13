using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ViVuStore.API.Configuration;
using ViVuStore.Business.Handlers;
using ViVuStore.Business.Services;
using ViVuStore.Data;
using ViVuStore.Data.Repositories;
using ViVuStore.Data.SeedData;
using ViVuStore.Data.UnitOfWorks;
using ViVuStore.Models.Security;
using ViVuStore.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add configuration sources including environment variables
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "ViVuStore Web API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter into field the word 'Bearer' following by space and JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});


// Register controllers
builder.Services.AddControllers();

// Register DBContext
var connectionString = builder.Configuration.GetConnectionString("ViVuStoreDbConnection");
builder.Services.AddDbContext<ViVuStoreDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });
});

// Register Identity: UserManager, RoleManager, SignInManager
builder.Services.AddIdentity<User, Role>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<ViVuStoreDbContext>()
    .AddDefaultTokenProviders();

// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register IUserIdentity to get current user
builder.Services.AddScoped<IUserIdentity, UserIdentity>();

// Register Token Service
builder.Services.AddScoped<ITokenService, TokenService>();

// Register MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CategoryCreateUpdateCommand).Assembly));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

// Register Versiong
builder.Services.AddVersionedApiExplorer(options =>
{
    // Add version 1.0 to the explorer
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Register JWT with Bearer token
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT:Secret is not configured.")))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", opt => opt
        .WithOrigins(builder.Configuration.GetSection("CORs:AllowedOrigins").Get<string[]>() ?? [])
        .WithHeaders(builder.Configuration.GetSection("CORs:AllowedHeaders").Get<string[]>() ?? [])
        .WithMethods(builder.Configuration.GetSection("CORs:AllowedMethods").Get<string[]>() ?? []));

    options.AddPolicy("AllowAnyOrigin", opt => opt
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ViVu Store Web API v1");
        options.EnableDeepLinking();
        options.DisplayRequestDuration();
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });

    // Seed data
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ViVuStoreDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

    // For Docker environment, add retry logic for migrations
    if (app.Environment.EnvironmentName == "Docker")
    {
        var retryCount = 0;
        const int maxRetries = 10;
        var delay = TimeSpan.FromSeconds(5);

        while (retryCount < maxRetries)
        {
            try
            {
                Console.WriteLine("Attempting to apply database migrations...");
                await context.Database.MigrateAsync();
                Console.WriteLine("Database migrations applied successfully.");
                break;
            }
            catch (Exception ex)
            {
                retryCount++;
                Console.WriteLine($"Failed to apply migrations: {ex.Message}. Retry {retryCount} of {maxRetries}.");
                if (retryCount >= maxRetries)
                {
                    throw;
                }
                await Task.Delay(delay);
            }
        }
    }
    else
    {
        // For development, just apply migrations directly
        await context.Database.MigrateAsync();
    }

    // Debug information about seed file paths
    var rolesJsonPath = Path.Combine(app.Environment.WebRootPath, "data", "roles.json");
    var usersJsonPath = Path.Combine(app.Environment.WebRootPath, "data", "users.json");
    
    Console.WriteLine($"WebRootPath: {app.Environment.WebRootPath}");
    Console.WriteLine($"Roles JSON path: {rolesJsonPath}");
    Console.WriteLine($"Users JSON path: {usersJsonPath}");
    Console.WriteLine($"Roles file exists: {File.Exists(rolesJsonPath)}");
    Console.WriteLine($"Users file exists: {File.Exists(usersJsonPath)}");
    
    // Ensure the directory exists
    Directory.CreateDirectory(Path.Combine(app.Environment.WebRootPath, "data"));
    
    // Attempt to seed the database
    try
    {
        DbInitializer.Seed(context, userManager, roleManager, rolesJsonPath, usersJsonPath);
        Console.WriteLine("Database seeded successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding database: {ex.Message}");
    }
}

// Add custom exception handler middleware
app.UseCustomExceptionHandler();

// Modify HTTPS redirection to be conditional based on environment
if (!app.Environment.EnvironmentName.Equals("Docker", StringComparison.OrdinalIgnoreCase))
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAnyOrigin");

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
