using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Business.ViewModels;
using ViVuStore.Core.Exceptions;
using ViVuStore.Data.UnitOfWorks;
using ViVuStore.Models.Common;

namespace ViVuStore.Business.Handlers;

public class CategoryCreateUpdateCommand : BaseCreateUpdateCommand<CategoryViewModel>
{
    [Required(ErrorMessage = "The {0} field is required.")]
    [StringLength(255,
        ErrorMessage = "The {0} field must be at least {2} and at max {1} characters long.",
        MinimumLength = 3)]
    public required string Name { get; set; }

    [StringLength(500,
        ErrorMessage = "The {0} field must be at least {2} and at max {1} characters long.",
        MinimumLength = 3)]
    public string? Description { get; set; }
}

public class CategoryCreateUpdateCommandHandler :
BaseHandler, IRequestHandler<CategoryCreateUpdateCommand, CategoryViewModel>
{
    public CategoryCreateUpdateCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    public Task<CategoryViewModel> Handle(
        CategoryCreateUpdateCommand request, CancellationToken cancellationToken)
    {
        if (request.Id.HasValue)
        {
            return Update(request);
        }
        else
        {
            return Create(request);
        }
    }

    private async Task<CategoryViewModel> Create(CategoryCreateUpdateCommand request)
    {
        var existedCategory = await _unitOfWork.CategoryRepository.GetQuery()
            .FirstOrDefaultAsync(x => x.Name == request.Name);

        if (existedCategory != null)
        {
            throw new ResourceUniqueException($"Category with name {request.Name} is already existed");
        }

        var entity = new Category
        {
            Name = request.Name,
            Description = request.Description
        };

        _unitOfWork.CategoryRepository.Add(entity);
        var result = await _unitOfWork.SaveChangesAsync();

        if (result <= 0)
        {
            throw new DatabaseBadRequestException("Create category failed");
        }

        return new CategoryViewModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy!.DisplayName ?? "",
            UpdatedAt = entity.UpdatedAt,
            UpdatedBy = entity.UpdatedBy!.DisplayName ?? "",
            DeletedAt = entity.DeletedAt,
            DeletedBy = entity.DeletedBy!.DisplayName ?? "",
            IsDeleted = entity.IsDeleted,
            IsActive = entity.IsActive
        };
    }

    private async Task<CategoryViewModel> Update(CategoryCreateUpdateCommand request)
    {
        var entity = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id!.Value) ??
            throw new ResourceNotFoundException($"Category with {request.Id} is not found");

        entity.Name = request.Name;
        entity.Description = request.Description;

        _unitOfWork.CategoryRepository.Update(entity);
        var result = await _unitOfWork.SaveChangesAsync();

        if (result <= 0)
        {
            throw new DatabaseBadRequestException("Update category failed");
        }

        return new CategoryViewModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy!.DisplayName ?? "",
            UpdatedAt = entity.UpdatedAt,
            UpdatedBy = entity.UpdatedBy!.DisplayName ?? "",
            DeletedAt = entity.DeletedAt,
            DeletedBy = entity.DeletedBy!.DisplayName ?? "",
            IsDeleted = entity.IsDeleted,
            IsActive = entity.IsActive
        };
    }
}
