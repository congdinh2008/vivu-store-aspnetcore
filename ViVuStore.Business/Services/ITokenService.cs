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
    
    /// <summary>
    /// Generates a refresh token for the specified user
    /// </summary>
    /// <param name="userId">The user ID to generate a refresh token for</param>
    /// <returns>Generated refresh token</returns>
    Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId);
    
    /// <summary>
    /// Validates a refresh token
    /// </summary>
    /// <param name="token">The refresh token to validate</param>
    /// <returns>The refresh token entity if valid, null otherwise</returns>
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);
    
    /// <summary>
    /// Revokes all refresh tokens for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="reason">The reason for revocation</param>
    Task RevokeUserRefreshTokensAsync(Guid userId, string reason);
    
    /// <summary>
    /// Revokes a specific refresh token
    /// </summary>
    /// <param name="token">The refresh token</param>
    /// <param name="replacedByToken">The new token replacing this one</param>
    /// <param name="reason">The reason for revocation</param>
    Task RevokeRefreshTokenAsync(RefreshToken token, string? replacedByToken, string reason);
}
