namespace Shared.Models.Responses;

public class SessionParticipantResponseDto
{
    public int Id { get; init; }

    public required QuizSessionResponseDto QuizSession { get; init; }

    public required AppUserResponseDto User { get; init; }

    public required string Nickname { get; init; }

    public DateTime JoinedAt { get; init; }

    public int TotalScore { get; init; } = 0;

    public bool IsConnected { get; init; } = true;

    // public string? ConnectionId { get; init; }

    public bool IsHost { get; init; } = false;

    public ICollection<ParticipantAnswerResponseDto> ParticipantAnswers { get; init; } = new List<ParticipantAnswerResponseDto>();
}