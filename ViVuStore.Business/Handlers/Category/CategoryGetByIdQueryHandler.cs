using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Business.ViewModels;
using ViVuStore.Core.Exceptions;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class CategoryGetByIdQueryHandler(IUnitOfWork unitOfWork) : BaseHandler(unitOfWork),
    IRequestHandler<CategoryGetByIdQuery, CategoryViewModel>
{
    public async Task<CategoryViewModel> Handle(
        CategoryGetByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.CategoryRepository.GetQuery()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken) ??
            throw new ResourceNotFoundException("Category not found");

        return new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt,
            CreatedBy = category.CreatedBy != null ? category.CreatedBy.DisplayName : "",
            UpdatedAt = category.UpdatedAt,
            UpdatedBy = category.UpdatedBy != null ? category.UpdatedBy.DisplayName : "",
            DeletedAt = category.DeletedAt,
            DeletedBy = category.DeletedBy != null ? category.DeletedBy.DisplayName : "",
            IsDeleted = category.IsDeleted,
            IsActive = category.IsActive
        };
    }
}