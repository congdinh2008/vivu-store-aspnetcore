using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ViVuStore.Business.Services;
using ViVuStore.Business.ViewModels.Auth;
using ViVuStore.Data.UnitOfWorks;
using ViVuStore.Models.Security;

namespace ViVuStore.Business.Handlers.Auth;

public class RefreshTokenCommandHandler : BaseHandler, IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public RefreshTokenCommandHandler(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        ITokenService tokenService, 
        UserManager<User> userManager,
        IConfiguration configuration) : base(unitOfWork, mapper)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Validate refresh token
        var refreshTokenEntity = await _tokenService.ValidateRefreshTokenAsync(request.RefreshToken);
        if (refreshTokenEntity == null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        // Get the user
        var user = refreshTokenEntity.User;
        
        // Check if user is active
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("User account is deactivated");
        }
        
        // Get user roles
        var userRoles = await _userManager.GetRolesAsync(user);
        
        // Generate new access token
        var accessToken = await _tokenService.GenerateTokenAsync(user, userRoles);

        // Generate new refresh token (token rotation for security)
        var newRefreshTokenEntity = await _tokenService.GenerateRefreshTokenAsync(user.Id);
        
        // Revoke current refresh token
        await _tokenService.RevokeRefreshTokenAsync(
            refreshTokenEntity, 
            newRefreshTokenEntity.Token, 
            "Replaced by new token");
        
        // Get token expiration
        if(!int.TryParse(_configuration["JWT:AccessTokenExpiryMinutes"], out var expiryMinutes))
        {
            expiryMinutes = 15;
        }
        
        // Return response
        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshTokenEntity.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };
    }
}
