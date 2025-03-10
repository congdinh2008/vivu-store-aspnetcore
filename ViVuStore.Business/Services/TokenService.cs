using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ViVuStore.Models.Security;

namespace ViVuStore.Business.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
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

        // Set token expiration (e.g., 1 day)
        var expiry = DateTime.UtcNow.AddDays(1);

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
}
