
using Microsoft.EntityFrameworkCore.Storage;
using ViVuStore.Data.Repositories;
using ViVuStore.Models;
using ViVuStore.Models.Common;
using ViVuStore.Models.Security;

namespace ViVuStore.Data.UnitOfWorks;

public class UnitOfWork : IUnitOfWork
{
    private readonly ViVuStoreDbContext _context;

    private readonly IUserIdentity _currentUser;
    
    private bool _disposed = false;

    public UnitOfWork(ViVuStoreDbContext context, IUserIdentity currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public ViVuStoreDbContext Context => _context;

    #region Implementation of Master Data Repositories
    private IMasterDataRepository<User>? _userRepository;
    public IMasterDataRepository<User> UserRepository => _userRepository ??= new MasterDataRepository<User>(_context, _currentUser);

    private IMasterDataRepository<Role>? _roleRepository;
    public IMasterDataRepository<Role> RoleRepository => _roleRepository ??= new MasterDataRepository<Role>(_context, _currentUser);

    private IMasterDataRepository<Category>? _categoryRepository;
    public IMasterDataRepository<Category> CategoryRepository => _categoryRepository ??= new MasterDataRepository<Category>(_context, _currentUser);

    #endregion

    #region Implementation of Repositories

    private IRepository<RefreshToken>? _refreshTokenRepository;

    public IRepository<RefreshToken> RefreshTokenRepository => _refreshTokenRepository ??= new Repository<RefreshToken>(_context, _currentUser);

    public IRepository<T> Repository<T>() where T : BaseEntity, IBaseEntity
    {
        return new Repository<T>(_context, _currentUser);
    }

    #endregion
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~UnitOfWork()
    {
        Dispose(false);
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }
}