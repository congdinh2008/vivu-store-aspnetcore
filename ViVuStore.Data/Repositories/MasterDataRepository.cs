using ViVuStore.Core.Constants;
using ViVuStore.Models;

namespace ViVuStore.Data.Repositories;

public class MasterDataRepository<T> : MasterDataRepositoryBase<T, ViVuStoreDbContext> where T : class,
        IMasterDataBaseEntity
{
    private readonly IUserIdentity _currentUser;

    public MasterDataRepository(ViVuStoreDbContext dataContext, IUserIdentity currentUser)
        : base(dataContext)
    {
        _currentUser = currentUser;
    }

    protected override Guid CurrentUserId
    {
        get
        {
            if (_currentUser != null)
            {
                return _currentUser.UserId;
            }

            return CoreConstants.SystemAdministratorId;
        }
    }

    protected override string CurrentUserName
    {
        get
        {
            if (_currentUser != null)
            {
                return _currentUser.UserName;
            }

            return CoreConstants.UserRoles.SystemAdministrator;
        }
    }
}
