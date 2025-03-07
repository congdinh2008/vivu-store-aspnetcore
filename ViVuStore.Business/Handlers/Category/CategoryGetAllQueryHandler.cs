using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Business.ViewModels;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class CategoryGetAllQueryHandler(IUnitOfWork unitOfWork) : BaseHandler(unitOfWork),
    IRequestHandler<CategoryGetAllQuery, IEnumerable<CategoryViewModel>>
{
    public async Task<IEnumerable<CategoryViewModel>> Handle(
        CategoryGetAllQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.CategoryRepository
            .GetQuery().Include(x => x.CreatedBy).ToListAsync(cancellationToken);

        return categories.Select(x => new CategoryViewModel
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
    }
}
