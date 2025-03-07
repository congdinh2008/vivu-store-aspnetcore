using MediatR;
using ViVuStore.Business.ViewModels;

namespace ViVuStore.Business.Handlers;

public class CategoryGetAllQuery : IRequest<IEnumerable<CategoryViewModel>>
{

}
