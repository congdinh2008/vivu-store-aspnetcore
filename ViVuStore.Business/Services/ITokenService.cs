using ViVuStore.Models.Security;

namespace ViVuStore.Business.Services;

public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token for the specified user
    /// </summary>
    /// <param name="user">The user to generate a token for</param>
    /// <param name="userRoles">The roles assigned to the user</param>
    /// <returns>Generated JWT token as string</returns>
    Task<string> GenerateTokenAsync(User user, IList<string> userRoles);
}
