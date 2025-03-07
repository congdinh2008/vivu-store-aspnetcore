using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ViVuStore.Business.Handlers;
using ViVuStore.Data;
using ViVuStore.Data.Repositories;
using ViVuStore.Data.SeedData;
using ViVuStore.Data.UnitOfWorks;
using ViVuStore.Models.Security;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddDbContext<ViVuStoreDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ViVuStoreDbConnection"));
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

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CategoryCreateUpdateCommand).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
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

    var rolesJsonPath = Path.Combine(app.Environment.WebRootPath, "data", "roles.json");
    var usersJsonPath = Path.Combine(app.Environment.WebRootPath, "data", "users.json");
    DbInitializer.Seed(context, userManager, roleManager, rolesJsonPath, usersJsonPath);
}

app.UseHttpsRedirection();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
