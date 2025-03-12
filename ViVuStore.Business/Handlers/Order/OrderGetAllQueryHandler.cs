using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViVuStore.Business.ViewModels;
using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class OrderGetAllQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : 
    BaseHandler(unitOfWork, mapper),
    IRequestHandler<OrderGetAllQuery, IEnumerable<OrderViewModel>>
{
    public async Task<IEnumerable<OrderViewModel>> Handle(OrderGetAllQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.OrderRepository.GetQuery();
        
        // Filter by user if specified
        if (request.UserId.HasValue)
        {
            query = query.Where(o => o.UserId == request.UserId.Value);
        }
        
        var orders = await query
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<OrderViewModel>>(orders);
    }
}
