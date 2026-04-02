using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Request;

public class SessionParticipantRequestDto
{
    public int QuizSessionId { get; init; }

    public required string UserId { get; init; }

    [MaxLength(50, ErrorMessage = "Nickname must be less than 50 characters.")]
    public required string Nickname { get; init; }

    public DateTime JoinedAt { get; init; } = DateTime.UtcNow;

    public int TotalScore { get; init; } = 0;

    public bool IsConnected { get; init; } = true;

    // public string? ConnectionId { get; set; }

    public bool IsHost { get; init; } = false;
}