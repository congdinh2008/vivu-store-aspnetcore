namespace ViVuStore.Business.ViewModels;

public class ProductViewModel : MasterBaseViewModel
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int UnitInStock { get; set; }

    public string? Thumbnail { get; set; }

    public bool IsDiscontinued { get; set; }

    // Relationships
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }
}
