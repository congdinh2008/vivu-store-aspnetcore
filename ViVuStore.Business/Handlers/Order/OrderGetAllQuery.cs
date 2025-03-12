using MediatR;
using ViVuStore.Business.ViewModels;

namespace ViVuStore.Business.Handlers;

public class OrderGetAllQuery : IRequest<IEnumerable<OrderViewModel>>
{
    public Guid? UserId { get; set; }
}
