using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Business.ViewModels;
using ViVuStore.Core.Exceptions;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class CategoryGetByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : 
    BaseHandler(unitOfWork, mapper),
    IRequestHandler<CategoryGetByIdQuery, CategoryViewModel>
{
    public async Task<CategoryViewModel> Handle(
        CategoryGetByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.CategoryRepository.GetQuery()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken) ??
            throw new ResourceNotFoundException("Category not found");

        return _mapper.Map<CategoryViewModel>(category);
    }
}