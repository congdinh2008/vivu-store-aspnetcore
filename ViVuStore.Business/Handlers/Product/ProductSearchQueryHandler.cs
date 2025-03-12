using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuLMS.Core;
using ViVuStore.Business.ViewModels;
using ViVuStore.Core.Extensions;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class ProductSearchQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : 
    BaseHandler(unitOfWork, mapper),
    IRequestHandler<ProductSearchQuery, PaginatedResult<ProductViewModel>>
{
    public async Task<PaginatedResult<ProductViewModel>> Handle(
        ProductSearchQuery request,
        CancellationToken cancellationToken)
    {
        // Create query
        var query = _unitOfWork.ProductRepository.GetQuery(request.IncludeInactive ?? false);

        // Filter by keyword
        if (!string.IsNullOrEmpty(request.Keyword))
        {
            query = query.Where(x => x.Name.Contains(request.Keyword) || 
                                     (x.Description != null && x.Description.Contains(request.Keyword)));
        }

        // Filter by category
        if (request.CategoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == request.CategoryId.Value);
        }

        // Filter by supplier
        if (request.SupplierId.HasValue)
        {
            query = query.Where(x => x.SupplierId == request.SupplierId.Value);
        }

        // Filter by price range
        if (request.MinPrice.HasValue)
        {
            query = query.Where(x => x.Price >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(x => x.Price <= request.MaxPrice.Value);
        }

        // Filter by discontinued status
        if (request.IsDiscontinued.HasValue)
        {
            query = query.Where(x => x.IsDiscontinued == request.IsDiscontinued.Value);
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
            query = query.OrderBy(x => x.Name);
        }

        // Get data with pagination
        var items = await query.Skip(request.PageSize * (request.PageNumber - 1))
            .Take(request.PageSize)
            .Include(x => x.Category)
            .Include(x => x.Supplier)
            .Include(x => x.CreatedBy)
            .Include(x => x.UpdatedBy)
            .Include(x => x.DeletedBy)
            .ToListAsync(cancellationToken);

        // Map to view models
        var viewModels = _mapper.Map<IEnumerable<ProductViewModel>>(items);

        // Return paginated result
        return new PaginatedResult<ProductViewModel>(request.PageNumber, request.PageSize, total, viewModels);
    }
}
