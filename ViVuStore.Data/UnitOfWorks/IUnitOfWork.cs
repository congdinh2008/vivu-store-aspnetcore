using Microsoft.EntityFrameworkCore.Storage;
using ViVuStore.Data.Repositories;
using ViVuStore.Models;
using ViVuStore.Models.Common;
using ViVuStore.Models.Security;

namespace ViVuStore.Data.UnitOfWorks;
public interface IUnitOfWork : IDisposable
{
    ViVuStoreDbContext Context { get; }

    #region Master Data Repositories
    IMasterDataRepository<User> UserRepository { get; }

    IMasterDataRepository<Role> RoleRepository { get; }

    IMasterDataRepository<Category> CategoryRepository { get; }

    IMasterDataRepository<Supplier> SupplierRepository { get; }

    IMasterDataRepository<Product> ProductRepository { get; }

    IRepository<RefreshToken> RefreshTokenRepository { get; }
    #endregion

    #region Repositories

    IRepository<T> Repository<T>() where T : BaseEntity, IBaseEntity;

    #endregion

    int SaveChanges();

    Task<int> SaveChangesAsync();

    Task<IDbContextTransaction> BeginTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();
}
