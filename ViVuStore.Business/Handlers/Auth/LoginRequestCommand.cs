using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using ViVuStore.Business.ViewModels.Auth;

namespace ViVuStore.Business.Handlers.Auth;

public class LoginRequestCommand: IRequest<LoginResponse>
{
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(50, ErrorMessage = "{0} must be at least {2} and at max {1} characters long", MinimumLength = 3)]
    public required string Username { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    [StringLength(20, ErrorMessage = "{0} must be at least {2} and at max {1} characters long", MinimumLength = 8)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}
