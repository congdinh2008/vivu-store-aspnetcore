using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViVuLMS.Core;
using ViVuStore.Business.Handlers;
using ViVuStore.Business.ViewModels;

namespace ViVuStore.API.Controllers;

/// <summary>
/// API controller for managing products.
/// </summary>
[Produces("application/json")]
[ApiController]
[Route("api/v{version:apiVersion}/products")]
[ApiVersion("1.0")]
[Authorize(Roles = "System Administrator, Administrator")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Retrieves all products.
    /// </summary>
    /// <returns>A collection of all products.</returns>
    /// <response code="200">Returns the list of products</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync()
    {
        var query = new ProductGetAllQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Searches products with pagination based on query parameters.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PaginatedResult<ProductViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAsync(
        [FromQuery] string? keyword,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string orderBy = "CreatedAt",
        [FromQuery] OrderDirection orderDirection = OrderDirection.ASC,
        [FromQuery] bool? includeInactive = false,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] Guid? supplierId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] bool? isDiscontinued = null)
    {
        var query = new ProductSearchQuery
        {
            Keyword = keyword,
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrderBy = orderBy,
            OrderDirection = orderDirection,
            IncludeInactive = includeInactive,
            CategoryId = categoryId,
            SupplierId = supplierId,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            IsDiscontinued = isDiscontinued
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Searches products with pagination based on request body.
    /// </summary>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<ProductViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAsync([FromBody] ProductSearchQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific product by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>The requested product.</returns>
    [HttpGet("{id}", Name = "GetProductById")]
    [ProducesResponseType(typeof(ProductViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var query = new ProductGetByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="command">The product creation data.</param>
    /// <returns>The newly created product.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProductViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(ProductCreateUpdateCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="id">The unique identifier of the product to update.</param>
    /// <param name="command">The updated product data.</param>
    /// <returns>The updated product.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProductViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, ProductCreateUpdateCommand command)
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
    /// Deletes a product by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the product to delete.</param>
    /// <returns>A success status</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new ProductDeleteByIdCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
