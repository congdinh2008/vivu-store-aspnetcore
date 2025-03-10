using System.ComponentModel.DataAnnotations;
using MediatR;
using ViVuStore.Business.ViewModels.Auth;

namespace ViVuStore.Business.Handlers.Auth;

public class RefreshTokenCommand : IRequest<LoginResponse>
{
    [Required]
    public required string RefreshToken { get; set; }
}
