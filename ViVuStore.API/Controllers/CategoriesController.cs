using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViVuStore.Business.Handlers;

namespace ViVuStore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> GetAllAsync()
    {
        var query = new CategoryGetAllQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var query = new CategoryGetByIdQuery(){
            Id = id
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryCreateUpdateCommand command)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CategoryCreateUpdateCommand command)
    {
        command.Id = id;

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
