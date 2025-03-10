using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ViVuStore.Data.UnitOfWorks;
using ViVuStore.Models.Security;

namespace ViVuStore.Business.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _key;
    private readonly IUnitOfWork _unitOfWork;

    public TokenService(IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? "congdinh2012@hotmail.com"));
    }

    public async Task<string> GenerateTokenAsync(User user, IList<string> userRoles)
    {
        // Create claims for the token
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new("fullName", user.DisplayName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add role claims
        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Create credentials using the secret key
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

        // Get token expiration from config (default to 15 minutes)
        if (!int.TryParse(_configuration["JWT:AccessTokenExpiryMinutes"], out var expiryMinutes))
        {
            expiryMinutes = 15;
        }
        
        // Set token expiration
        var expiry = DateTime.UtcNow.AddMinutes(expiryMinutes);

        // Create the JWT token
        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        // Return the serialized token
        return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
    
    public async Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId)
    {
        // Generate a random token using cryptographically secure random number generator
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        
        // Convert to base64 string
        var refreshToken = Convert.ToBase64String(randomBytes);
        
        // Get refresh token expiration from config (default to 7 days)
        if(!int.TryParse(_configuration["JWT:RefreshTokenExpiryDays"], out var expiryDays)){
            expiryDays = 7;
        }
        
        // Create refresh token entity
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(expiryDays),
            UserId = userId
        };
        
        // Save to database
        _unitOfWork.RefreshTokenRepository.Add(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();
        
        return refreshTokenEntity;
    }
    
    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
    {
        var refreshToken = await _unitOfWork.Context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == token);
        
        return refreshToken?.IsActive == true ? refreshToken : null;
    }
    
    public async Task RevokeUserRefreshTokensAsync(Guid userId, string reason)
    {
        // Get all active refresh tokens for the user
        var activeTokens = await _unitOfWork.Context.RefreshTokens
            .Where(r => r.UserId == userId && !r.IsRevoked)
            .ToListAsync();
        
        // Revoke all tokens
        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
            token.ReasonRevoked = reason;
        }
        
        await _unitOfWork.SaveChangesAsync();
    }
    
    public async Task RevokeRefreshTokenAsync(RefreshToken token, string? replacedByToken, string reason)
    {
        token.IsRevoked = true;
        token.ReasonRevoked = reason;
        token.ReplacedByToken = replacedByToken;
        
        await _unitOfWork.SaveChangesAsync();
    }
}
