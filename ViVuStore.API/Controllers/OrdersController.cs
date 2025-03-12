using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViVuLMS.Core;
using ViVuStore.Business.Handlers;
using ViVuStore.Business.ViewModels;
using ViVuStore.Data.Repositories;

namespace ViVuStore.API.Controllers;

/// <summary>
/// API controller for managing orders.
/// </summary>
[Produces("application/json")]
[ApiController]
[Route("api/v{version:apiVersion}/orders")]
[ApiVersion("1.0")]
[Authorize]
public class OrdersController(IMediator mediator, IUserIdentity currentUser) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IUserIdentity _currentUser = currentUser;

    /// <summary>
    /// Retrieves all orders (admin only).
    /// </summary>
    /// <returns>A collection of all orders.</returns>
    [HttpGet]
    [Authorize(Roles = "System Administrator, Administrator")]
    [ProducesResponseType(typeof(IEnumerable<OrderViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync()
    {
        var query = new OrderGetAllQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    /// <summary>
    /// Retrieves all orders for the current user.
    /// </summary>
    /// <returns>A collection of orders for the current user.</returns>
    [HttpGet("my-orders")]
    [ProducesResponseType(typeof(IEnumerable<OrderViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyOrdersAsync()
    {
        var query = new OrderGetAllQuery { UserId = _currentUser.UserId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Searches orders with pagination (admin only).
    /// </summary>
    [HttpGet("search")]
    [Authorize(Roles = "System Administrator, Administrator")]
    [ProducesResponseType(typeof(PaginatedResult<OrderViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAsync(
        [FromQuery] string? keyword,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string orderBy = "OrderDate",
        [FromQuery] OrderDirection orderDirection = OrderDirection.DESC,
        [FromQuery] bool? includeInactive = false,
        [FromQuery] Guid? userId = null,
        [FromQuery] DateTime? fromOrderDate = null,
        [FromQuery] DateTime? toOrderDate = null,
        [FromQuery] bool? hasShipped = null)
    {
        var query = new OrderSearchQuery
        {
            Keyword = keyword,
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrderBy = orderBy,
            OrderDirection = orderDirection,
            IncludeInactive = includeInactive,
            UserId = userId,
            FromOrderDate = fromOrderDate,
            ToOrderDate = toOrderDate,
            HasShipped = hasShipped
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Searches the current user's orders with pagination.
    /// </summary>
    [HttpGet("my-orders/search")]
    [ProducesResponseType(typeof(PaginatedResult<OrderViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchMyOrdersAsync(
        [FromQuery] string? keyword,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string orderBy = "OrderDate",
        [FromQuery] OrderDirection orderDirection = OrderDirection.DESC,
        [FromQuery] DateTime? fromOrderDate = null,
        [FromQuery] DateTime? toOrderDate = null,
        [FromQuery] bool? hasShipped = null)
    {
        var query = new OrderSearchQuery
        {
            Keyword = keyword,
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrderBy = orderBy,
            OrderDirection = orderDirection,
            UserId = _currentUser.UserId, // Filter for current user
            FromOrderDate = fromOrderDate,
            ToOrderDate = toOrderDate,
            HasShipped = hasShipped
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Searches orders with pagination based on request body (admin only).
    /// </summary>
    [HttpPost("search")]
    [Authorize(Roles = "System Administrator, Administrator")]
    [ProducesResponseType(typeof(PaginatedResult<OrderViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAsync([FromBody] OrderSearchQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific order by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the order.</param>
    /// <returns>The requested order.</returns>
    [HttpGet("{id}", Name = "GetOrderById")]
    [ProducesResponseType(typeof(OrderViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var query = new OrderGetByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        
        // Ensure users can only see their own orders unless they are administrators
        if (result.UserId != _currentUser.UserId && !User.IsInRole("System Administrator") && !User.IsInRole("Administrator"))
        {
            return Forbid();
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Creates a new order for the current user.
    /// </summary>
    /// <param name="command">The order creation data.</param>
    /// <returns>The newly created order.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(OrderViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(OrderCreateCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _mediator.Send(command);
        return CreatedAtRoute("GetOrderById", new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing order.
    /// </summary>
    /// <param name="id">The unique identifier of the order to update.</param>
    /// <param name="command">The updated order data.</param>
    /// <returns>The updated order.</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "System Administrator, Administrator")]
    [ProducesResponseType(typeof(OrderViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, OrderUpdateCommand command)
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
    /// Cancels an order by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the order to delete.</param>
    /// <returns>A success status</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id)
    {
        // Check if order belongs to current user if not admin
        if (!User.IsInRole("System Administrator") && !User.IsInRole("Administrator"))
        {
            var query = new OrderGetByIdQuery { Id = id };
            try
            {
                var order = await _mediator.Send(query);
                if (order.UserId != _currentUser.UserId)
                {
                    return Forbid();
                }
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        
        var command = new OrderDeleteByIdCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
