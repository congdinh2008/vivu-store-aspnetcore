using MediatR;

namespace ViVuStore.Business.Handlers;

public class BaseGetAllQuery<T> : 
    IRequest<IEnumerable<T>> where T : class
{
}
