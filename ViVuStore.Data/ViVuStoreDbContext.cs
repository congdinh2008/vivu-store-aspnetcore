using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Core.Constants;
using ViVuStore.Models;
using ViVuStore.Models.Common;
using ViVuStore.Models.Security;

namespace ViVuStore.Data;

public class ViVuStoreDbContext: IdentityDbContext<User, Role, Guid>
{
    public ViVuStoreDbContext(DbContextOptions<ViVuStoreDbContext> options): base(options)
    {
        
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Rename Identity tables
        builder.Entity<User>().ToTable("Users", CoreConstants.Schemas.Security);
        builder.Entity<Role>().ToTable("Roles", CoreConstants.Schemas.Security);
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles", CoreConstants.Schemas.Security);
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims", CoreConstants.Schemas.Security);
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins", CoreConstants.Schemas.Security);
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims", CoreConstants.Schemas.Security);
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens", CoreConstants.Schemas.Security);

        // Configure RefreshToken relationship
        builder.Entity<RefreshToken>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Global query filter for soft delete
        builder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Role>().HasQueryFilter(x => !x.IsDeleted);
    }

    public override int SaveChanges()
    {
        BeforeSaveChange();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        BeforeSaveChange();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void BeforeSaveChange()
    {
        var entities = this.ChangeTracker.Entries<IBaseEntity>();

        foreach (var item in entities)
        {
            switch (item.State)
            {
                case EntityState.Added:
                    item.Entity.CreatedAt = DateTime.Now;
                    break;
                case EntityState.Modified:
                    item.Entity.UpdatedAt = DateTime.Now;
                    break;
            }
        }
    }
}
