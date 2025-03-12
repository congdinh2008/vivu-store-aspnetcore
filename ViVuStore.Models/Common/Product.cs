using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViVuStore.Models.Common;

[Table("Products", Schema = "Common")]
public class Product : MasterDataBaseEntity, IMasterDataBaseEntity
{
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Required]
    public decimal Price { get; set; }

    [Required]
    public int UnitInStock { get; set; }

    [StringLength(2000)]
    public string? Thumbnail { get; set; }

    public bool IsDiscontinued { get; set; }

    // Foreign keys
    public Guid? CategoryId { get; set; }

    public Guid? SupplierId { get; set; }

    // Navigation properties
    [ForeignKey(nameof(CategoryId))]
    public virtual Category? Category { get; set; }

    [ForeignKey(nameof(SupplierId))]
    public virtual Supplier? Supplier { get; set; }
}
