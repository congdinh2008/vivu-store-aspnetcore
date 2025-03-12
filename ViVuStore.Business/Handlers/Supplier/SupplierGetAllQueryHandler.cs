using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Business.ViewModels;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class SupplierGetAllQueryHandler : BaseHandler,
    IRequestHandler<SupplierGetAllQuery, IEnumerable<SupplierViewModel>>
{
    public SupplierGetAllQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
    }

    public async Task<IEnumerable<SupplierViewModel>> Handle(
        SupplierGetAllQuery request,
        CancellationToken cancellationToken)
    {
        var suppliers = await _unitOfWork.SupplierRepository
            .GetQuery().Include(x => x.CreatedBy).ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<SupplierViewModel>>(suppliers);
    }
}
