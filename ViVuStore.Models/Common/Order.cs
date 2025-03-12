using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ViVuStore.Models.Security;

namespace ViVuStore.Models.Common;

[Table("Orders", Schema = "Common")]
public class Order : MasterDataBaseEntity, IMasterDataBaseEntity
{
    [Required]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(500)]
    public required string ShippedAddress { get; set; }

    public DateTime? ExpectedShippedDate { get; set; }

    public DateTime? ActualShippedDate { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }
    
    // Foreign key for the user who placed the order
    public Guid UserId { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }
    
    // Collection of order details
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
