using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Business.ViewModels;
using ViVuStore.Core.Exceptions;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class OrderGetByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : 
    BaseHandler(unitOfWork, mapper),
    IRequestHandler<OrderGetByIdQuery, OrderViewModel>
{
    public async Task<OrderViewModel> Handle(OrderGetByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.GetQuery()
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken) ??
            throw new ResourceNotFoundException("Order not found");

        return _mapper.Map<OrderViewModel>(order);
    }
}
