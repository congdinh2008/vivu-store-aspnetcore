using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViVuStore.Business.Handlers.Auth;

namespace ViVuStore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
