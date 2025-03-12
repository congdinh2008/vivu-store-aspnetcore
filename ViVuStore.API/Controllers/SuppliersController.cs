using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViVuLMS.Core;
using ViVuStore.Business.Handlers;
using ViVuStore.Business.ViewModels;

namespace ViVuStore.API.Controllers;

/// <summary>
/// API controller for managing suppliers.
/// </summary>
[Produces("application/json")]
[ApiController]
[Route("api/v{version:apiVersion}/suppliers")]
[ApiVersion("1.0")]
[Authorize(Roles = "System Administrator, Administrator")]
public class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SuppliersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all suppliers.
    /// </summary>
    /// <returns>A collection of all suppliers.</returns>
    /// <response code="200">Returns the list of suppliers</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SupplierViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync()
    {
        var query = new SupplierGetAllQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Searches suppliers with pagination based on query parameters.
    /// </summary>
    /// <param name="keyword">Search keyword</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10)</param>
    /// <param name="orderBy">Field to order by (default: CreatedAt)</param>
    /// <param name="orderDirection">Order direction (ASC or DESC)</param>
    /// <param name="includeInactive">Whether to include inactive suppliers</param>
    /// <returns>A paginated list of suppliers matching the search criteria</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PaginatedResult<SupplierViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAsync(
        [FromQuery] string keyword,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string orderBy = "CreatedAt",
        [FromQuery] OrderDirection orderDirection = OrderDirection.ASC,
        [FromQuery] bool? includeInactive = false)
    {
        var query = new SupplierSearchQuery
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

    /// <summary>
    /// Searches suppliers with pagination based on request body.
    /// </summary>
    /// <param name="query">Search parameters</param>
    /// <returns>A paginated list of suppliers matching the search criteria</returns>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<SupplierViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAsync([FromBody] SupplierSearchQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific supplier by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the supplier.</param>
    /// <returns>The requested supplier.</returns>
    /// <response code="200">Returns the requested supplier</response>
    /// <response code="404">If the supplier is not found</response>
    [HttpGet("{id}", Name = "GetSupplierById")]
    [ProducesResponseType(typeof(SupplierViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var query = new SupplierGetByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new supplier.
    /// </summary>
    /// <param name="command">The supplier creation data.</param>
    /// <returns>The newly created supplier.</returns>
    /// <response code="200">Returns the newly created supplier</response>
    /// <response code="400">If the supplier data is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(SupplierViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(SupplierCreateUpdateCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing supplier.
    /// </summary>
    /// <param name="id">The unique identifier of the supplier to update.</param>
    /// <param name="command">The updated supplier data.</param>
    /// <returns>The updated supplier.</returns>
    /// <response code="200">Returns the updated supplier</response>
    /// <response code="400">If the supplier data is invalid</response>
    /// <response code="404">If the supplier is not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SupplierViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, SupplierCreateUpdateCommand command)
    {
        command.Id = id;

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a supplier by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the supplier to delete.</param>
    /// <returns>A success status</returns>
    /// <response code="200">If deletion was successful</response>
    /// <response code="404">If the supplier is not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new SupplierDeleteByIdCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
