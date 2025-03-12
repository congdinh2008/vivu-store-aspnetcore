namespace ViVuStore.Business.ViewModels;

public class OrderDetailViewModel
{
    public Guid OrderId { get; set; }
    
    public Guid ProductId { get; set; }
    
    public string? ProductName { get; set; }
    
    public string? ProductThumbnail { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal Price { get; set; }
    
    public decimal Discount { get; set; }
    
    public decimal SubTotal => Quantity * Price * (1 - Discount);
}
