using System.ComponentModel.DataAnnotations;
using MediatR;
using ViVuStore.Business.ViewModels;

namespace ViVuStore.Business.Handlers;

public class OrderCreateCommand : IRequest<OrderViewModel>
{
    [Required(ErrorMessage = "The {0} field is required.")]
    [StringLength(500, ErrorMessage = "The {0} field must be at most {1} characters long.")]
    public required string ShippedAddress { get; set; }
    
    public DateTime? ExpectedShippedDate { get; set; }
    
    [Phone(ErrorMessage = "The {0} field is not a valid phone number.")]
    [StringLength(20, ErrorMessage = "The {0} field must be at most {1} characters long.")]
    public string? PhoneNumber { get; set; }
    
    [Required(ErrorMessage = "At least one product is required")]
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}

public class OrderItemDto
{
    [Required]
    public Guid ProductId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
}
