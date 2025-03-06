using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViVuStore.Models.Common;

[Table("Categories", Schema = "Common")]
public class Category: MasterDataBaseEntity, IMasterDataBaseEntity
{
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}
