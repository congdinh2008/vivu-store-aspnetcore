using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Business.ViewModels;
using ViVuStore.Core.Exceptions;
using ViVuStore.Data.Repositories;
using ViVuStore.Data.UnitOfWorks;
using ViVuStore.Models.Common;
using ViVuStore.Models.Security;

namespace ViVuStore.Business.Handlers;

public class OrderCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserIdentity currentUser) : 
    BaseHandler(unitOfWork, mapper),
    IRequestHandler<OrderCreateCommand, OrderViewModel>
{
    private readonly IUserIdentity _currentUser = currentUser;

    public async Task<OrderViewModel> Handle(OrderCreateCommand request, CancellationToken cancellationToken)
    {
        if (request.Items == null || !request.Items.Any())
        {
            throw new ValidationException("At least one product is required for an order");
        }

        // Get current user
        var user = await _unitOfWork.UserRepository.GetByIdAsync(_currentUser.UserId) ??
            throw new ResourceNotFoundException($"User with ID {_currentUser.UserId} not found");

        // Begin transaction
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Create order
            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                ShippedAddress = request.ShippedAddress,
                ExpectedShippedDate = request.ExpectedShippedDate,
                PhoneNumber = request.PhoneNumber,
                UserId = user.Id
            };

            _unitOfWork.OrderRepository.Add(order);
            await _unitOfWork.SaveChangesAsync();

            // Process order items
            foreach (var item in request.Items)
            {
                // Get product
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId) ??
                    throw new ResourceNotFoundException($"Product with ID {item.ProductId} not found");

                // Check stock
                if (product.UnitInStock < item.Quantity)
                {
                    throw new ValidationException($"Not enough stock for product {product.Name}. Available: {product.UnitInStock}, Requested: {item.Quantity}");
                }

                // Create order detail
                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Price = product.Price,
                    Discount = 0 // Default to no discount
                };

                _unitOfWork.OrderDetailRepository.Add(orderDetail);

                // Update product stock
                product.UnitInStock -= item.Quantity;
                _unitOfWork.ProductRepository.Update(product);
            }

            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();

            // Retrieve the complete order with details
            var completedOrder = await _unitOfWork.OrderRepository.GetQuery()
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == order.Id, cancellationToken);

            return _mapper.Map<OrderViewModel>(completedOrder);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
