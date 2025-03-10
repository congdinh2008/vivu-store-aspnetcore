using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuLMS.Core;
using ViVuStore.Business.ViewModels;
using ViVuStore.Core.Extensions;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class CategorySeachQueryHandler(IUnitOfWork unitOfWork) : BaseHandler(unitOfWork),
    IRequestHandler<CategorySearchQuery, PaginatedResult<CategoryViewModel>>
{
    public async Task<PaginatedResult<CategoryViewModel>> Handle(
        CategorySearchQuery request,
        CancellationToken cancellationToken)
    {
        // Tao query
        var query = _unitOfWork.CategoryRepository.GetQuery(request.IncludeInactive ?? false);

        // Check keyword not null or empty, then filter
        if (!string.IsNullOrEmpty(request.Keyword))
        {
            query = query.Where(x => x.Name.Contains(request.Keyword) || x.Description!.Contains(request.Keyword));
        }

        // Dem so luong
        int total = await query.CountAsync(cancellationToken);

        // Sap xep
        if (!string.IsNullOrEmpty(request.OrderBy))
        {
            query = query.OrderByExtension(request.OrderBy, request.OrderDirection.ToString());
        }
        else
        {
            query = query.OrderBy(x => x.Name);
        }

        // Lay du lieu
        var items = await query.Skip(request.PageSize * (request.PageNumber - 1))
            .Take(request.PageSize)
            .Include(x => x.CreatedBy)
            .Include(x => x.UpdatedBy)
            .Include(x => x.DeletedBy)
            .ToListAsync(cancellationToken);

        // Chuyen du lieu sang view model
        var viewModels = items.Select(x => new CategoryViewModel
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            CreatedAt = x.CreatedAt,
            CreatedBy = x.CreatedBy != null ? x.CreatedBy.DisplayName : "",
            UpdatedAt = x.UpdatedAt,
            UpdatedBy = x.UpdatedBy != null ? x.UpdatedBy.DisplayName : "",
            DeletedAt = x.DeletedAt,
            DeletedBy = x.DeletedBy != null ? x.DeletedBy.DisplayName : "",
            IsDeleted = x.IsDeleted,
            IsActive = x.IsActive
        });

        // Tra ve ket qua
        return new PaginatedResult<CategoryViewModel>(request.PageNumber, request.PageSize, total, viewModels);
    }
}
