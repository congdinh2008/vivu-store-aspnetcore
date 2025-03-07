namespace ViVuStore.Business.ViewModels;

public class CategoryViewModel: MasterBaseViewModel
{
    public required string Name { get; set; }

    public string? Description { get; set; }
}
