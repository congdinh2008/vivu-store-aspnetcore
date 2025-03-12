using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Business.ViewModels;
using ViVuStore.Core.Exceptions;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class OrderUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : 
    BaseHandler(unitOfWork, mapper),
    IRequestHandler<OrderUpdateCommand, OrderViewModel>
{
    public async Task<OrderViewModel> Handle(OrderUpdateCommand request, CancellationToken cancellationToken)
    {
        // Get the order
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.Id) ??
            throw new ResourceNotFoundException($"Order with ID {request.Id} not found");

        // Update order details
        order.ShippedAddress = request.ShippedAddress;
        order.ExpectedShippedDate = request.ExpectedShippedDate;
        order.ActualShippedDate = request.ActualShippedDate;
        order.PhoneNumber = request.PhoneNumber;

        // Save changes
        _unitOfWork.OrderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync();

        // Retrieve updated order with details
        var updatedOrder = await _unitOfWork.OrderRepository.GetQuery()
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.Id == order.Id, cancellationToken);

        return _mapper.Map<OrderViewModel>(updatedOrder);
    }
}
