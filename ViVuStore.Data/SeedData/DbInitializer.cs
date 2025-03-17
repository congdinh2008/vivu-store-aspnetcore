using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using ViVuStore.Core.Constants;
using ViVuStore.Models.Security;
using ViVuStore.Models.Common;

namespace ViVuStore.Data.SeedData;

public static class DbInitializer
{
    public static void Seed(ViVuStoreDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager,
        string rolesJsonPath, string usersJsonPath)
    {
        context.Database.EnsureCreated();

        string jsonRoles = File.ReadAllText(rolesJsonPath);
        var roles = JsonConvert.DeserializeObject<List<Role>>(jsonRoles);

        string jsonUsers = File.ReadAllText(usersJsonPath);
        var users = JsonConvert.DeserializeObject<List<UserJsonViewModel>>(jsonUsers);

        if (roles == null || users == null)
        {
            return;
        }

        SeedUserAndRoles(userManager, roleManager, users, roles);

        // Get the admin user for setting creator information
        var adminUser = userManager.FindByNameAsync("systemadministrator").Result;
        if (adminUser != null)
        {
            // Seed categories, suppliers, and products 
            SeedCategories(context, adminUser.Id);
            SeedSuppliers(context, adminUser.Id);
            SeedProducts(context, adminUser.Id);
        }

        context.SaveChanges();
    }

    private static void SeedUserAndRoles(UserManager<User> userManager, RoleManager<Role> roleManager, List<UserJsonViewModel> users, List<Role> roles)
    {
        if (!userManager.Users.Any(x => x.UserName == "systemadministrator"))
        {
            if (users == null)
            {
                return;
            }

            var passwordHash = new PasswordHasher<User>();

            foreach (var user in users)
            {
                var newUser = new User
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = true,
                    DateOfBirth = DateTime.Parse(user.DateOfBirth).ToUniversalTime(),
                    IsActive = true,
                };

                if (user.Role == "System Administrator")
                {
                    newUser.Id = CoreConstants.SystemAdministratorId;
                }

                string password = passwordHash.HashPassword(newUser, user.Password);
                newUser.PasswordHash = password;

                // check if system administrator exists
                var systemAdministrator = userManager.FindByNameAsync("systemadministrator").Result;
                if (systemAdministrator != null)
                {
                    newUser.CreatedById = systemAdministrator.Id;
                }

                var result = userManager.CreateAsync(newUser, user.Password).Result;

                if (result.Succeeded)
                {
                    var userRole = roleManager.FindByNameAsync(user.Role).Result;

                    if (userRole == null)
                    {
                        var newRole = roles.FirstOrDefault(x => x.Name == user.Role);
                        if (newRole == null)
                        {
                            continue;
                        }

                        if (systemAdministrator != null)
                        {
                            newRole.CreatedById = systemAdministrator.Id;
                        }
                        roleManager.CreateAsync(newRole).Wait();
                    }

                    var result2 = userManager.AddToRoleAsync(newUser, user.Role).Result;

                    if (!result2.Succeeded)
                    {
                        continue;
                    }
                }
            }
        }
    }

    private static void SeedCategories(ViVuStoreDbContext context, Guid createdById)
    {
        // Check if categories already exist
        if (context.Set<Category>().Any())
        {
            return;
        }

        string categoriesJsonPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "wwwroot", "data", "categories.json");

        if (File.Exists(categoriesJsonPath))
        {
            string jsonCategories = File.ReadAllText(categoriesJsonPath);
            var categories = JsonConvert.DeserializeObject<List<CategoryJsonViewModel>>(jsonCategories);

            if (categories != null)
            {
                foreach (var category in categories)
                {
                    context.Set<Category>().Add(new Category
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedById = createdById,
                    });
                }

                context.SaveChanges();
            }
        }
    }

    private static void SeedSuppliers(ViVuStoreDbContext context, Guid createdById)
    {
        // Check if suppliers already exist
        if (context.Set<Supplier>().Any())
        {
            return;
        }

        string suppliersJsonPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "wwwroot", "data", "suppliers.json");

        if (File.Exists(suppliersJsonPath))
        {
            string jsonSuppliers = File.ReadAllText(suppliersJsonPath);
            var suppliers = JsonConvert.DeserializeObject<List<SupplierJsonViewModel>>(jsonSuppliers);

            if (suppliers != null)
            {
                foreach (var supplier in suppliers)
                {
                    context.Set<Supplier>().Add(new Supplier
                    {
                        Id = supplier.Id,
                        Name = supplier.Name,
                        Address = supplier.Address,
                        PhoneNumber = supplier.PhoneNumber,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedById = createdById,
                    });
                }

                context.SaveChanges();
            }
        }
    }

    private static void SeedProducts(ViVuStoreDbContext context, Guid createdById)
    {
        // Check if products already exist
        if (context.Set<Product>().Any())
        {
            return;
        }

        string productsJsonPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "wwwroot", "data", "products.json");

        if (File.Exists(productsJsonPath))
        {
            string jsonProducts = File.ReadAllText(productsJsonPath);
            var products = JsonConvert.DeserializeObject<List<ProductJsonViewModel>>(jsonProducts);

            if (products != null)
            {
                foreach (var product in products)
                {
                    context.Set<Product>().Add(new Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        UnitInStock = product.UnitInStock,
                        Thumbnail = product.Thumbnail,
                        IsDiscontinued = product.IsDiscontinued,
                        CategoryId = product.CategoryId,
                        SupplierId = product.SupplierId,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedById = createdById,
                    });
                }

                context.SaveChanges();
            }
        }
    }
}

internal class UserJsonViewModel
{
    public Guid Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string UserName { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }

    public required string PhoneNumber { get; set; }

    public required string DateOfBirth { get; set; }

    public required string Role { get; set; }
}

internal class CategoryJsonViewModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}

internal class SupplierJsonViewModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
}

internal class ProductJsonViewModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int UnitInStock { get; set; }
    public string? Thumbnail { get; set; }
    public bool IsDiscontinued { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? SupplierId { get; set; }
}