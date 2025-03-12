using ViVuStore.Business.ViewModels;

namespace ViVuStore.Business.Handlers;

public class OrderSearchQuery : MasterDataSearchQuery<OrderViewModel>
{
    public Guid? UserId { get; set; }
    
    public DateTime? FromOrderDate { get; set; }
    
    public DateTime? ToOrderDate { get; set; }
    
    public bool? HasShipped { get; set; }
}
