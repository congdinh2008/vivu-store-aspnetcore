using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViVuStore.Business.Handlers;
using ViVuStore.Business.ViewModels;

namespace ViVuStore.API.Controllers;

/// <summary>
/// API controller for managing categories.
/// </summary>
[Produces("application/json")]
[ApiController]
[Route("api/v{version:apiVersion}/categories")]
[ApiVersion("1.0")]
[Authorize(Roles = "System Administrator, Administrator")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(IMediator mediator, ILogger<CategoriesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all categories.
    /// </summary>
    /// <returns>A collection of all categories.</returns>
    /// <response code="200">Returns the list of categories</response>
    [HttpGet()]
    [ProducesResponseType(typeof(IEnumerable<CategoryViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogInformation("Getting all categories");
        var query = new CategoryGetAllQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<CategoryViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAsync(
        [FromQuery] string keyword,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string orderBy = "CreatedAt",
        [FromQuery] OrderDirection orderDirection = OrderDirection.ASC,
        [FromQuery] bool? includeInactive = false
    )
    {
        var query = new CategorySearchQuery()
        {
            Keyword = keyword,
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrderBy = orderBy,
            OrderDirection = orderDirection,
            IncludeInactive = includeInactive
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(IEnumerable<CategoryViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAsync([FromBody] CategorySearchQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific category by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the category.</param>
    /// <returns>The requested category.</returns>
    /// <response code="200">Returns the requested category</response>
    /// <response code="404">If the category is not found</response>
    [HttpGet("{id}", Name = "GetCategoryById")]
    [ProducesResponseType(typeof(CategoryViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var query = new CategoryGetByIdQuery()
        {
            Id = id
        };
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    /// <param name="command">The category creation data.</param>
    /// <returns>The newly created category.</returns>
    /// <response code="200">Returns the newly created category</response>
    /// <response code="400">If the category data is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CategoryCreateUpdateCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    /// <param name="id">The unique identifier of the category to update.</param>
    /// <param name="command">The updated category data.</param>
    /// <returns>The updated category.</returns>
    /// <response code="200">Returns the updated category</response>
    /// <response code="400">If the category data is invalid</response>
    /// <response code="404">If the category is not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CategoryViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, CategoryCreateUpdateCommand command)
    {
        command.Id = id;

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _mediator.Send(command);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
