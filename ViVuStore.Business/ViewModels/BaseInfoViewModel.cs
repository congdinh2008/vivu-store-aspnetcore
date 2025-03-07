namespace ViVuStore.Business.ViewModels;

public class BaseInfoViewModel: BaseViewModel
{
    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public bool IsDeleted { get; set; }
}
