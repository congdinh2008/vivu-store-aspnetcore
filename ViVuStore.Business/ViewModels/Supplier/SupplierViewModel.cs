namespace ViVuStore.Business.ViewModels;

public class SupplierViewModel : MasterBaseViewModel
{
    public required string Name { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }
}
