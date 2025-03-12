using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViVuStore.Models.Common;

[Table("Suppliers", Schema = "Common")]
public class Supplier : MasterDataBaseEntity, IMasterDataBaseEntity
{
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }
}
