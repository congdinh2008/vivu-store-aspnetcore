using System.ComponentModel.DataAnnotations;
using ViVuStore.Business.ViewModels;

namespace ViVuStore.Business.Handlers;

public class ProductCreateUpdateCommand : BaseCreateUpdateCommand<ProductViewModel>
{
    [Required(ErrorMessage = "The {0} field is required.")]
    [StringLength(255,
        ErrorMessage = "The {0} field must be at least {2} and at max {1} characters long.",
        MinimumLength = 3)]
    public required string Name { get; set; }

    [StringLength(2000,
        ErrorMessage = "The {0} field must be at most {1} characters long.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "The {0} field is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "The {0} field is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Units in stock must be a non-negative number")]
    public int UnitInStock { get; set; }

    [StringLength(2000,
        ErrorMessage = "The {0} field must be at most {1} characters long.")]
    public string? Thumbnail { get; set; }

    public bool IsDiscontinued { get; set; }

    // Relationships
    public Guid? CategoryId { get; set; }

    public Guid? SupplierId { get; set; }
}
