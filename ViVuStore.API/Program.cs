using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Data;
using ViVuStore.Data.SeedData;
using ViVuStore.Models.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

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

await app.RunAsync();
