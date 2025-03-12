using AutoMapper;
using MediatR;
using ViVuStore.Core.Exceptions;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class ProductDeleteByIdCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) :
    BaseHandler(unitOfWork, mapper),
    IRequestHandler<ProductDeleteByIdCommand, bool>
{
    public async Task<bool> Handle(
        ProductDeleteByIdCommand request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id) ??
            throw new ResourceNotFoundException($"Product with ID {request.Id} was not found");

        _unitOfWork.ProductRepository.Delete(entity);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }
}
