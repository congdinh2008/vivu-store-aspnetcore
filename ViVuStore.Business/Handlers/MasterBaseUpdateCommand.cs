namespace ViVuStore.Business.Handlers;

public class MasterBaseUpdateCommand<T> : 
    BaseUpdateCommand<T> where T : class
{
    public bool IsActive { get; set; }
}