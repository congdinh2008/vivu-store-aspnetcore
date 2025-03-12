using ViVuStore.Business.ViewModels;

namespace ViVuStore.Business.Handlers;

public class ProductSearchQuery : MasterDataSearchQuery<ProductViewModel>
{
    public Guid? CategoryId { get; set; }
    public Guid? SupplierId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsDiscontinued { get; set; }
}
