namespace ViVuStore.Business.Handlers;

public class MasterBaseGetByIdQuery<T> : 
    BaseGetByIdQuery<T> where T : class
{
    public bool IncludeInactive { get; set; }
}
