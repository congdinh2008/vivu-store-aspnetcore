using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViVuStore.Business.Handlers.Auth;
using ViVuStore.Business.Services;

namespace ViVuStore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPost("revoke-token")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RevokeTokenAsync([FromBody] string refreshToken)
    {
        // Get the current user's ID from the claims
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Invalid token");
        }
        
        // Get the token service to revoke the refresh token
        var tokenService = HttpContext.RequestServices.GetRequiredService<ITokenService>();
        
        // Validate the refresh token
        var refreshTokenEntity = await tokenService.ValidateRefreshTokenAsync(refreshToken);
        if (refreshTokenEntity == null)
        {
            return BadRequest("Invalid refresh token");
        }
        
        // Check if the refresh token belongs to the current user
        if (refreshTokenEntity.UserId != Guid.Parse(userId))
        {
            return Unauthorized("Unauthorized");
        }
        
        // Revoke the refresh token
        await tokenService.RevokeRefreshTokenAsync(refreshTokenEntity, null, "Revoked by user");
        
        return Ok(new { message = "Token revoked" });
    }
}
