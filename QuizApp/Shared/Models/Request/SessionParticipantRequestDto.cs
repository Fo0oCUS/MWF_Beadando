using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Request;

public class SessionParticipantRequestDto
{
    public int QuizSessionId { get; init; }

    public string? UserId { get; init; }

    [MaxLength(50, ErrorMessage = "Nickname must be less than 50 characters.")]
    public required string Nickname { get; init; }

    public DateTime JoinedAt { get; init; } = DateTime.UtcNow;

    public bool IsConnected { get; init; } = true;

    // public string? ConnectionId { get; set; }
}