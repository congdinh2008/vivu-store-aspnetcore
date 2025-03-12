using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Core.Exceptions;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class OrderDeleteByIdCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : 
    BaseHandler(unitOfWork, mapper),
    IRequestHandler<OrderDeleteByIdCommand, bool>
{
    public async Task<bool> Handle(OrderDeleteByIdCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Get the order with its details
            var order = await _unitOfWork.OrderRepository.GetQuery()
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken) ??
                throw new ResourceNotFoundException($"Order with ID {request.Id} was not found");

            // Restore product quantities
            foreach (var detail in order.OrderDetails)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(detail.ProductId);
                if (product != null)
                {
                    product.UnitInStock += detail.Quantity;
                    _unitOfWork.ProductRepository.Update(product);
                }
            }

            // Delete the order (OrderDetails will be deleted via cascade)
            _unitOfWork.OrderRepository.Delete(order);
            
            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
