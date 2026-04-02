namespace Shared.Models.Responses;

public class QuestionResponseDto
{
    public int Id { get; init; }

    public required QuizResponseDto Quiz { get; init; }

    public required string Text { get; init; }

    public int OrderIndex { get; init; }

    public int TimeLimitSeconds { get; init; }

    public required ICollection<AnswerOptionResponseDto> AnswerOptions { get; init; }
}