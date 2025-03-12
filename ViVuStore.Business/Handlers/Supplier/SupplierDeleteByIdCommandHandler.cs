using AutoMapper;
using MediatR;
using ViVuStore.Core.Exceptions;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class SupplierDeleteByIdCommandHandler : BaseHandler,
    IRequestHandler<SupplierDeleteByIdCommand, bool>
{
    public SupplierDeleteByIdCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
    }

    public async Task<bool> Handle(
        SupplierDeleteByIdCommand request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.SupplierRepository.GetByIdAsync(request.Id) ??
            throw new ResourceNotFoundException($"Supplier with ID {request.Id} was not found");

        _unitOfWork.SupplierRepository.Delete(entity);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }
}
