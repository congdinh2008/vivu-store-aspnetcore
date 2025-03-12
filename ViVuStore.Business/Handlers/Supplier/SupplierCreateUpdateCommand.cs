using System.ComponentModel.DataAnnotations;
using ViVuStore.Business.ViewModels;

namespace ViVuStore.Business.Handlers;

public class SupplierCreateUpdateCommand : BaseCreateUpdateCommand<SupplierViewModel>
{
    [Required(ErrorMessage = "The {0} field is required.")]
    [StringLength(255,
        ErrorMessage = "The {0} field must be at least {2} and at max {1} characters long.",
        MinimumLength = 3)]
    public required string Name { get; set; }

    [StringLength(500,
        ErrorMessage = "The {0} field must be at most {1} characters long.")]
    public string? Address { get; set; }

    [StringLength(20,
        ErrorMessage = "The {0} field must be at most {1} characters long.")]
    [Phone(ErrorMessage = "The {0} field is not a valid phone number.")]
    public string? PhoneNumber { get; set; }
}
