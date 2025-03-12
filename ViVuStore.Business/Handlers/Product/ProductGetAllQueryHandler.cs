using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Business.ViewModels;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class ProductGetAllQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) :
    BaseHandler(unitOfWork, mapper),
    IRequestHandler<ProductGetAllQuery, IEnumerable<ProductViewModel>>
{
    public async Task<IEnumerable<ProductViewModel>> Handle(
        ProductGetAllQuery request,
        CancellationToken cancellationToken)
    {
        var products = await _unitOfWork.ProductRepository
            .GetQuery()
            .Include(x => x.Category)
            .Include(x => x.Supplier)
            .Include(x => x.CreatedBy)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<ProductViewModel>>(products);
    }
}
