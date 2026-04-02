namespace Shared.Models.Request;

public class ParticipantAnswerRequestDto
{
    public int SessionParticipantId { get; set; }

    public int QuestionId { get; set; }

    public int AnswerOptionId { get; set; }

    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

    public bool IsCorrect { get; set; }

    public int AwardedPoints { get; set; } = 1;

    public int ResponseTimeMs { get; set; }
}