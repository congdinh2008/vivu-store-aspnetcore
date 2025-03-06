namespace ViVuStore.Data.Repositories;

public interface IUserIdentity
{
    Guid UserId { get; }

    string UserName { get; }
}
