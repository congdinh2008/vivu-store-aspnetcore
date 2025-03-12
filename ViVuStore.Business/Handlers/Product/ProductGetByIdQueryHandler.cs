using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Business.ViewModels;
using ViVuStore.Core.Exceptions;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class ProductGetByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : 
    BaseHandler(unitOfWork, mapper),
    IRequestHandler<ProductGetByIdQuery, ProductViewModel>
{
    public async Task<ProductViewModel> Handle(
        ProductGetByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.ProductRepository.GetQuery()
            .Include(x => x.Category)
            .Include(x => x.Supplier)
            .Include(x => x.CreatedBy)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken) ??
            throw new ResourceNotFoundException("Product not found");

        return _mapper.Map<ProductViewModel>(product);
    }
}
