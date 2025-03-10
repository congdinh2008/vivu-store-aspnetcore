using System.ComponentModel.DataAnnotations.Schema;

namespace ViVuStore.Models.Security;

[Table("RefreshTokens", Schema = "Security")]
public class RefreshToken : BaseEntity
{
    public required string Token { get; set; }

    public DateTime ExpiryDate { get; set; }

    public bool IsUsed { get; set; }

    public bool IsRevoked { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;
    
    public string? ReplacedByToken { get; set; }
    
    public string? ReasonRevoked { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
    
    public bool IsActive => !IsRevoked && !IsExpired && !IsUsed;
}
