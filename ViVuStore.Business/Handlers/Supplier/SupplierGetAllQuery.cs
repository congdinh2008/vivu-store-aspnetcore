using MediatR;
using ViVuStore.Business.ViewModels;

namespace ViVuStore.Business.Handlers;

public class SupplierGetAllQuery : IRequest<IEnumerable<SupplierViewModel>>
{
}
