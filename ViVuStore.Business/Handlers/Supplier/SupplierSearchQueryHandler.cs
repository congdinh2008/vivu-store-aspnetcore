using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuLMS.Core;
using ViVuStore.Business.ViewModels;
using ViVuStore.Core.Extensions;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class SupplierSearchQueryHandler : BaseHandler,
    IRequestHandler<SupplierSearchQuery, PaginatedResult<SupplierViewModel>>
{
    public SupplierSearchQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
    }

    public async Task<PaginatedResult<SupplierViewModel>> Handle(
        SupplierSearchQuery request,
        CancellationToken cancellationToken)
    {
        // Create query
        var query = _unitOfWork.SupplierRepository.GetQuery(request.IncludeInactive ?? false);

        // Check if keyword is not null or empty, then filter
        if (!string.IsNullOrEmpty(request.Keyword))
        {
            query = query.Where(x => x.Name.Contains(request.Keyword) || 
                                    (x.Address != null && x.Address.Contains(request.Keyword)) || 
                                    (x.PhoneNumber != null && x.PhoneNumber.Contains(request.Keyword)));
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
            .Include(x => x.CreatedBy)
            .Include(x => x.UpdatedBy)
            .Include(x => x.DeletedBy)
            .ToListAsync(cancellationToken);

        // Map to view models
        var viewModels = _mapper.Map<IEnumerable<SupplierViewModel>>(items);

        // Return paginated result
        return new PaginatedResult<SupplierViewModel>(request.PageNumber, request.PageSize, total, viewModels);
    }
}
