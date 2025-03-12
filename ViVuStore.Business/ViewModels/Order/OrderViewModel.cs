using ViVuStore.Models.Security;

namespace ViVuStore.Business.ViewModels;

public class OrderViewModel : MasterBaseViewModel
{
    public DateTime OrderDate { get; set; }
    
    public required string ShippedAddress { get; set; }
    
    public DateTime? ExpectedShippedDate { get; set; }
    
    public DateTime? ActualShippedDate { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    // User information
    public Guid UserId { get; set; }
    
    public string? UserName { get; set; }
    
    // Order details
    public List<OrderDetailViewModel> OrderDetails { get; set; } = new List<OrderDetailViewModel>();
    
    // Calculated properties
    public decimal TotalAmount => OrderDetails.Sum(od => od.SubTotal);
    
    public int TotalItems => OrderDetails.Sum(od => od.Quantity);
}
