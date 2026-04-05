namespace Shared.Models.Responses;

public class QuizSummaryResponseDto
{
    public int Id { get; init; }

    public required string Title { get; init; }

    public bool IsPublished { get; init; }

    public int QuestionCount { get; init; }
}
