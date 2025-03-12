using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViVuStore.Models.Common;

[Table("OrderDetails", Schema = "Common")]
public class OrderDetail : BaseEntity, IBaseEntity
{
    [Required]
    public Guid OrderId { get; set; }
    
    [Required]
    public Guid ProductId { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    [Required]
    public decimal Price { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Discount { get; set; } = 0;
    
    // Navigation properties
    [ForeignKey(nameof(OrderId))]
    public virtual Order? Order { get; set; }
    
    [ForeignKey(nameof(ProductId))]
    public virtual Product? Product { get; set; }
    
    // Calculated property
    [NotMapped]
    public decimal SubTotal => Quantity * Price * (1 - Discount);
}
