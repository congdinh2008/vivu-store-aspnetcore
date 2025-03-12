using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuLMS.Core;
using ViVuStore.Business.ViewModels;
using ViVuStore.Core.Extensions;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class OrderSearchQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : 
    BaseHandler(unitOfWork, mapper),
    IRequestHandler<OrderSearchQuery, PaginatedResult<OrderViewModel>>
{
    public async Task<PaginatedResult<OrderViewModel>> Handle(OrderSearchQuery request, CancellationToken cancellationToken)
    {
        // Create query
        var query = _unitOfWork.OrderRepository.GetQuery(request.IncludeInactive ?? false);

        // Filter by user if specified
        if (request.UserId.HasValue)
        {
            query = query.Where(o => o.UserId == request.UserId.Value);
        }
        
        // Filter by order date range
        if (request.FromOrderDate.HasValue)
        {
            query = query.Where(o => o.OrderDate >= request.FromOrderDate.Value);
        }
        
        if (request.ToOrderDate.HasValue)
        {
            query = query.Where(o => o.OrderDate <= request.ToOrderDate.Value);
        }
        
        // Filter by shipped status
        if (request.HasShipped.HasValue)
        {
            query = query.Where(o => (o.ActualShippedDate != null) == request.HasShipped.Value);
        }
        
        // Filter by keyword
        if (!string.IsNullOrEmpty(request.Keyword))
        {
            query = query.Where(o => o.ShippedAddress.Contains(request.Keyword) || 
                                   (o.PhoneNumber != null && o.PhoneNumber.Contains(request.Keyword)));
        }

        // Count total items
        int total = await query.CountAsync(cancellationToken);

        // Sort
        if (!string.IsNullOrEmpty(request.OrderBy))
        {
            query = query.OrderByExtension(request.OrderBy, request.OrderDirection.ToString());
        }
        else
        {
            query = query.OrderByDescending(x => x.OrderDate);
        }

        // Get data with pagination
        var items = await query.Skip(request.PageSize * (request.PageNumber - 1))
            .Take(request.PageSize)
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .ToListAsync(cancellationToken);

        // Map to view models
        var viewModels = _mapper.Map<IEnumerable<OrderViewModel>>(items);

        // Return paginated result
        return new PaginatedResult<OrderViewModel>(request.PageNumber, request.PageSize, total, viewModels);
    }
}
