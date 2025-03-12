using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Business.ViewModels;
using ViVuStore.Core.Exceptions;
using ViVuStore.Data.UnitOfWorks;
using ViVuStore.Models.Common;

namespace ViVuStore.Business.Handlers;

public class ProductCreateUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) :
    BaseHandler(unitOfWork, mapper), 
    IRequestHandler<ProductCreateUpdateCommand, ProductViewModel>
{
    public Task<ProductViewModel> Handle(
        ProductCreateUpdateCommand request, CancellationToken cancellationToken)
    {
        if (request.Id.HasValue)
        {
            return Update(request, cancellationToken);
        }
        else
        {
            return Create(request, cancellationToken);
        }
    }

    private async Task<ProductViewModel> Create(ProductCreateUpdateCommand request, CancellationToken cancellationToken)
    {
        // Check if category exists
        if (request.CategoryId.HasValue)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.CategoryId.Value);
            if (category == null)
            {
                throw new ResourceNotFoundException($"Category with ID {request.CategoryId} was not found");
            }
        }

        // Check if supplier exists
        if (request.SupplierId.HasValue)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(request.SupplierId.Value);
            if (supplier == null)
            {
                throw new ResourceNotFoundException($"Supplier with ID {request.SupplierId} was not found");
            }
        }

        var entity = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            UnitInStock = request.UnitInStock,
            Thumbnail = request.Thumbnail,
            IsDiscontinued = request.IsDiscontinued,
            CategoryId = request.CategoryId,
            SupplierId = request.SupplierId
        };

        _unitOfWork.ProductRepository.Add(entity);
        var result = await _unitOfWork.SaveChangesAsync();

        if (result <= 0)
        {
            throw new DatabaseBadRequestException("Failed to create product");
        }

        var createdEntity = await _unitOfWork.ProductRepository.GetQuery()
            .Include(x => x.Category)
            .Include(x => x.Supplier)
            .Include(x => x.CreatedBy)
            .Include(x => x.UpdatedBy)
            .Include(x => x.DeletedBy)
            .FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken) ??
            throw new ResourceNotFoundException($"Product with ID {entity.Id} not found");

        return _mapper.Map<ProductViewModel>(createdEntity);
    }

    private async Task<ProductViewModel> Update(ProductCreateUpdateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id!.Value) ??
            throw new ResourceNotFoundException($"Product with ID {request.Id} not found");

        // Check if category exists
        if (request.CategoryId.HasValue)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.CategoryId.Value);
            if (category == null)
            {
                throw new ResourceNotFoundException($"Category with ID {request.CategoryId} was not found");
            }
        }

        // Check if supplier exists
        if (request.SupplierId.HasValue)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(request.SupplierId.Value);
            if (supplier == null)
            {
                throw new ResourceNotFoundException($"Supplier with ID {request.SupplierId} was not found");
            }
        }

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.UnitInStock = request.UnitInStock;
        entity.Thumbnail = request.Thumbnail;
        entity.IsDiscontinued = request.IsDiscontinued;
        entity.CategoryId = request.CategoryId;
        entity.SupplierId = request.SupplierId;

        _unitOfWork.ProductRepository.Update(entity);
        var result = await _unitOfWork.SaveChangesAsync();

        if (result <= 0)
        {
            throw new DatabaseBadRequestException("Update product failed");
        }

        var updatedEntity = await _unitOfWork.ProductRepository.GetQuery()
            .Include(x => x.Category)
            .Include(x => x.Supplier)
            .Include(x => x.CreatedBy)
            .Include(x => x.UpdatedBy)
            .Include(x => x.DeletedBy)
            .FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken) ??
            throw new ResourceNotFoundException($"Product with ID {entity.Id} not found");

        return _mapper.Map<ProductViewModel>(updatedEntity);
    }
}
