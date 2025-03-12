using System.ComponentModel.DataAnnotations;
using ViVuStore.Business.ViewModels;

namespace ViVuStore.Business.Handlers;

public class OrderUpdateCommand : BaseUpdateCommand<OrderViewModel>
{
    [Required(ErrorMessage = "The {0} field is required.")]
    [StringLength(500, ErrorMessage = "The {0} field must be at most {1} characters long.")]
    public required string ShippedAddress { get; set; }
    
    public DateTime? ExpectedShippedDate { get; set; }
    
    public DateTime? ActualShippedDate { get; set; }
    
    [Phone(ErrorMessage = "The {0} field is not a valid phone number.")]
    [StringLength(20, ErrorMessage = "The {0} field must be at most {1} characters long.")]
    public string? PhoneNumber { get; set; }
}
