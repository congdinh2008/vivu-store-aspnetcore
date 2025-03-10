using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Business.ViewModels;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class CategoryGetAllQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) :
    BaseHandler(unitOfWork, mapper),
    IRequestHandler<CategoryGetAllQuery, IEnumerable<CategoryViewModel>>
{
    public async Task<IEnumerable<CategoryViewModel>> Handle(
        CategoryGetAllQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.CategoryRepository
            .GetQuery().Include(x => x.CreatedBy).ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<CategoryViewModel>>(categories);
    }
}
