using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Business.ViewModels;
using ViVuStore.Core.Exceptions;
using ViVuStore.Data.UnitOfWorks;
using ViVuStore.Models.Common;

namespace ViVuStore.Business.Handlers;

public class CategoryCreateUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) :
    BaseHandler(unitOfWork, mapper), 
    IRequestHandler<CategoryCreateUpdateCommand, CategoryViewModel>
{
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

        return _mapper.Map<CategoryViewModel>(createdEntity);
    }

    private async Task<CategoryViewModel> Update(CategoryCreateUpdateCommand request)
    {
        var entity = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id!.Value) ??
            throw new ResourceNotFoundException($"Category with {request.Id} is not found");

        _mapper.Map(request, entity);

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

        return _mapper.Map<CategoryViewModel>(updatedEntity);
    }
}
