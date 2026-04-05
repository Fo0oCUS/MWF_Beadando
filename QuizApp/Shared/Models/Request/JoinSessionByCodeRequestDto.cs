using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Request;

public class JoinSessionByCodeRequestDto
{
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Join code must be 6 characters long.")]
    public required string JoinCode { get; init; }

    [MaxLength(50, ErrorMessage = "Nickname must be less than 50 characters.")]
    [MinLength(2, ErrorMessage = "Nickname must be at least 2 characters long.")]
    public required string Nickname { get; init; }
}
