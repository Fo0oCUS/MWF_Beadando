namespace Shared.Models.Responses;

public class ParticipantAnswerResponseDto
{
    public int Id { get; init; }

    public required SessionParticipantResponseDto SessionParticipant { get; init; }

    public required QuestionResponseDto Question { get; init; }

    public required AnswerOptionResponseDto AnswerOption { get; init; }

    public DateTime AnsweredAt { get; init; } = DateTime.UtcNow;

    public bool IsCorrect { get; init; }

    public int AwardedPoints { get; init; }

    public int ResponseTimeMs { get; init; }
}