using System.Linq;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ViVuStore.Business.Services;
using ViVuStore.Business.ViewModels.Auth;
using ViVuStore.Core.Constants;
using ViVuStore.Data.UnitOfWorks;
using ViVuStore.Models.Security;
using static ViVuStore.Core.Constants.CoreConstants;

namespace ViVuStore.Business.Handlers.Auth;

public class RegisterRequestCommandHandler : BaseHandler, IRequestHandler<RegisterRequestCommand, LoginResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public RegisterRequestCommandHandler(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        UserManager<User> userManager, 
        ITokenService tokenService,
        IConfiguration configuration) : base(unitOfWork, mapper)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<LoginResponse> Handle(RegisterRequestCommand request, CancellationToken cancellationToken)
    {
        // Check if user with this username or email already exists
        var existingUserByName = await _userManager.FindByNameAsync(request.Username);
        if (existingUserByName != null)
        {
            throw new InvalidOperationException("Username already exists");
        }
        
        var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingUserByEmail != null)
        {
            throw new InvalidOperationException("Email already exists");
        }
        
        // Create new user
        var user = new User
        {
            UserName = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth ?? DateTime.UtcNow,
            Address = request.Address,
            IsActive = true,
            EmailConfirmed = true // In production, implement email confirmation
        };
        
        // Add the user using UserManager
        var result = await _userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            throw new InvalidOperationException($"Failed to create user: {string.Join(", ", errors)}");
        }
        
        // Assign default role
        await _userManager.AddToRoleAsync(user, RoleConstants.User);
        
        // Get user roles
        var userRoles = await _userManager.GetRolesAsync(user);
        
        // Generate access token
        var accessToken = await _tokenService.GenerateTokenAsync(user, userRoles);
        
        // Generate refresh token
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);
        
        // Get token expiration from config
        if (!int.TryParse(_configuration["JWT:AccessTokenExpiryMinutes"], out var expiryMinutes))
        {
            expiryMinutes = 15;
        }
        
        // Return login response
        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };
    }
}
