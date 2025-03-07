using ViVuStore.Data.UnitOfWorks;

namespace ViVuStore.Business.Handlers;

public class BaseHandler(IUnitOfWork unitOfWork)
{
    protected readonly IUnitOfWork _unitOfWork = unitOfWork;
}