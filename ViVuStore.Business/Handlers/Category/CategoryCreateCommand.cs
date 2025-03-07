using System.ComponentModel.DataAnnotations;
using ViVuStore.Business.ViewModels;

namespace ViVuStore.Business.Handlers;

public class CategoryCreateUpdateCommand : BaseCreateUpdateCommand<CategoryViewModel>
{
    [Required(ErrorMessage = "The {0} field is required.")]
    [StringLength(255,
        ErrorMessage = "The {0} field must be at least {2} and at max {1} characters long.",
        MinimumLength = 3)]
    public required string Name { get; set; }

    [StringLength(500,
        ErrorMessage = "The {0} field must be at least {2} and at max {1} characters long.",
        MinimumLength = 3)]
    public string? Description { get; set; }
}
