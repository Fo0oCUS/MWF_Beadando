namespace Shared.Models.Request;

public class SubmitParticipantAnswerRequestDto
{
    public int SessionParticipantId { get; init; }

    public int AnswerOptionId { get; init; }

    public int ResponseTimeMs { get; init; }
}
