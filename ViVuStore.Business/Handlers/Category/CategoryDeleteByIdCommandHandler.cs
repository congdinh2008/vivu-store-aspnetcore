using AutoMapper;
using MediatR;
using ViVuStore.Core.Exceptions;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class CategoryDeleteByIdCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) :
    BaseHandler(unitOfWork, mapper),
    IRequestHandler<CategoryDeleteByIdCommand, bool>
{
    public async Task<bool> Handle(
        CategoryDeleteByIdCommand request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id) ??
            throw new ResourceNotFoundException($"Category with {request.Id} is not found");

        _unitOfWork.CategoryRepository.Delete(entity);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }
}