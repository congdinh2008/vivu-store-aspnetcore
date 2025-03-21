using MediatR;

namespace ViVuStore.Business.Handlers;

public class BaseGetByIdQuery<T>: IRequest<T> where T : class
{
    public Guid Id { get; set; }
}
