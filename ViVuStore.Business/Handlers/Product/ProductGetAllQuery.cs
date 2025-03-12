using MediatR;
using ViVuStore.Business.ViewModels;

namespace ViVuStore.Business.Handlers;

public class ProductGetAllQuery : IRequest<IEnumerable<ProductViewModel>>
{
}
