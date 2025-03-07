using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViVuStore.Business.Handlers;

namespace ViVuStore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> GetAllAsync()
    {
        var query = new CategoryGetAllQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
