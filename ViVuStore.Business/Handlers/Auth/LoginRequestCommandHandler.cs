using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ViVuStore.Business.Services;
using ViVuStore.Business.ViewModels.Auth;
using ViVuStore.Data.UnitOfWorks;
using ViVuStore.Models.Security;

namespace ViVuStore.Business.Handlers.Auth;

public class LoginRequestCommandHandler :
    BaseHandler, 
    IRequestHandler<LoginRequestCommand, LoginResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;

    public LoginRequestCommandHandler(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        UserManager<User> userManager, 
        SignInManager<User> signInManager,
        ITokenService tokenService) : 
        base(unitOfWork, mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> Handle(LoginRequestCommand request, CancellationToken cancellationToken)
    {
        // Find user by username
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        // Check if the user is active
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Your account is deactivated. Please contact an administrator.");
        }

        // Check if the password is correct
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        // Get user roles
        var userRoles = await _userManager.GetRolesAsync(user);

        // Generate access token
        var accessToken = await _tokenService.GenerateTokenAsync(user, userRoles);

        // Return response
        return new LoginResponse
        {
            AccessToken = accessToken
        };
    }
}