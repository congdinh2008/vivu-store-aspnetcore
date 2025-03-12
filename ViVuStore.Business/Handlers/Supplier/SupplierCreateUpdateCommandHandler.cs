using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Business.ViewModels;
using ViVuStore.Core.Exceptions;
using ViVuStore.Data.UnitOfWorks;
using ViVuStore.Models.Common;

namespace ViVuStore.Business.Handlers;

public class SupplierCreateUpdateCommandHandler : BaseHandler,
    IRequestHandler<SupplierCreateUpdateCommand, SupplierViewModel>
{
    public SupplierCreateUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
    }

    public Task<SupplierViewModel> Handle(
        SupplierCreateUpdateCommand request, CancellationToken cancellationToken)
    {
        return request.Id.HasValue 
            ? Update(request, cancellationToken) 
            : Create(request, cancellationToken);
    }

    private async Task<SupplierViewModel> Create(SupplierCreateUpdateCommand request, CancellationToken cancellationToken)
    {
        var existingSupplier = await _unitOfWork.SupplierRepository.GetQuery()
            .FirstOrDefaultAsync(x => x.Name == request.Name, cancellationToken);

        if (existingSupplier != null)
        {
            throw new ResourceUniqueException($"Supplier with name '{request.Name}' already exists");
        }

        var entity = new Supplier
        {
            Name = request.Name,
            Address = request.Address,
            PhoneNumber = request.PhoneNumber
        };

        _unitOfWork.SupplierRepository.Add(entity);
        var result = await _unitOfWork.SaveChangesAsync();

        if (result <= 0)
        {
            throw new DatabaseBadRequestException("Failed to create supplier");
        }

        var createdEntity = await _unitOfWork.SupplierRepository.GetQuery()
            .Include(x => x.CreatedBy)
            .Include(x => x.UpdatedBy)
            .Include(x => x.DeletedBy)
            .FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken) ??
            throw new ResourceNotFoundException($"Supplier with ID {entity.Id} not found");

        return _mapper.Map<SupplierViewModel>(createdEntity);
    }

    private async Task<SupplierViewModel> Update(SupplierCreateUpdateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.SupplierRepository.GetByIdAsync(request.Id!.Value) ??
            throw new ResourceNotFoundException($"Supplier with ID {request.Id} not found");

        // Check if another supplier with the same name exists (only if name changed)
        if (entity.Name != request.Name)
        {
            var existingSupplier = await _unitOfWork.SupplierRepository.GetQuery()
                .FirstOrDefaultAsync(x => x.Name == request.Name && x.Id != request.Id, cancellationToken);

            if (existingSupplier != null)
            {
                throw new ResourceUniqueException($"Another supplier with name '{request.Name}' already exists");
            }
        }

        entity.Name = request.Name;
        entity.Address = request.Address;
        entity.PhoneNumber = request.PhoneNumber;

        _unitOfWork.SupplierRepository.Update(entity);
        var result = await _unitOfWork.SaveChangesAsync();

        if (result <= 0)
        {
            throw new DatabaseBadRequestException("Failed to update supplier");
        }

        var updatedEntity = await _unitOfWork.SupplierRepository.GetQuery()
            .Include(x => x.CreatedBy)
            .Include(x => x.UpdatedBy)
            .Include(x => x.DeletedBy)
            .FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken) ??
            throw new ResourceNotFoundException($"Supplier with ID {entity.Id} not found");

        return _mapper.Map<SupplierViewModel>(updatedEntity);
    }
}
