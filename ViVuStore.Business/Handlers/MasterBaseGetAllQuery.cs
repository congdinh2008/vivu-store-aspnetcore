namespace ViVuStore.Business.Handlers;

public class MasterBaseGetAllQuery<T> :
    BaseGetAllQuery<T> where T : class
{
    public bool IncludeInactive { get; set; }
}
