using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Business.ViewModels;
using ViVuStore.Core.Exceptions;
using ViVuStore.Data.UnitOfWorks;
using ViVuStore.Models.Common;

namespace ViVuStore.Business.Handlers;

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

        var createdEntity = await _unitOfWork.CategoryRepository.GetQuery()
            .Include(x => x.CreatedBy)
            .Include(x => x.UpdatedBy)
            .Include(x => x.DeletedBy)
            .FirstOrDefaultAsync(x => x.Id == entity.Id) ??
            throw new ResourceNotFoundException($"Category with {entity.Id} is not found");

        return new CategoryViewModel
        {
            Id = createdEntity.Id,
            Name = createdEntity.Name,
            Description = createdEntity.Description,
            CreatedAt = createdEntity.CreatedAt,
            CreatedBy = createdEntity.CreatedBy != null ? createdEntity.CreatedBy.DisplayName : "",
            UpdatedAt = createdEntity.UpdatedAt,
            UpdatedBy = createdEntity.UpdatedBy != null ? createdEntity.UpdatedBy.DisplayName : "",
            DeletedAt = createdEntity.DeletedAt,
            DeletedBy = createdEntity.DeletedBy != null ? createdEntity.DeletedBy.DisplayName : "",
            IsDeleted = createdEntity.IsDeleted,
            IsActive = createdEntity.IsActive
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

        var updatedEntity = await _unitOfWork.CategoryRepository.GetQuery()
            .Include(x => x.CreatedBy)
            .Include(x => x.UpdatedBy)
            .Include(x => x.DeletedBy)
            .FirstOrDefaultAsync(x => x.Id == entity.Id) ??
            throw new ResourceNotFoundException($"Category with {entity.Id} is not found");

        return new CategoryViewModel
        {
            Id = updatedEntity.Id,
            Name = updatedEntity.Name,
            Description = updatedEntity.Description,
            CreatedAt = updatedEntity.CreatedAt,
            CreatedBy = updatedEntity.CreatedBy != null ? updatedEntity.CreatedBy.DisplayName : "",
            UpdatedAt = updatedEntity.UpdatedAt,
            UpdatedBy = updatedEntity.UpdatedBy != null ? updatedEntity.UpdatedBy.DisplayName : "",
            DeletedAt = updatedEntity.DeletedAt,
            DeletedBy = updatedEntity.DeletedBy != null ? updatedEntity.DeletedBy.DisplayName : "",
            IsDeleted = updatedEntity.IsDeleted,
            IsActive = updatedEntity.IsActive
        };
    }
}
