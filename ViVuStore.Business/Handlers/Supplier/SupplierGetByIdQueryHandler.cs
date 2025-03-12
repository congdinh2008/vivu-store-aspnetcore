using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Business.ViewModels;
using ViVuStore.Core.Exceptions;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class SupplierGetByIdQueryHandler : BaseHandler,
    IRequestHandler<SupplierGetByIdQuery, SupplierViewModel>
{
    public SupplierGetByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
    }

    public async Task<SupplierViewModel> Handle(
        SupplierGetByIdQuery request,
        CancellationToken cancellationToken)
    {
        var supplier = await _unitOfWork.SupplierRepository.GetQuery()
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken) ??
            throw new ResourceNotFoundException("Supplier not found");

        return _mapper.Map<SupplierViewModel>(supplier);
    }
}
