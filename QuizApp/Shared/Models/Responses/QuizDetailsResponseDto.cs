namespace Shared.Models.Responses;

public class QuizDetailsResponseDto
{
    public int Id { get; init; }

    public required string Title { get; init; }

    public bool IsPublished { get; init; }

    public required ICollection<QuizQuestionDetailsResponseDto> Questions { get; init; }
}

public class QuizQuestionDetailsResponseDto
{
    public int Id { get; init; }

    public required string Text { get; init; }

    public int OrderIndex { get; init; }

    public int TimeLimitSeconds { get; init; }

    public required ICollection<QuizAnswerOptionDetailsResponseDto> AnswerOptions { get; init; }
}

public class QuizAnswerOptionDetailsResponseDto
{
    public int Id { get; init; }

    public required string Text { get; init; }

    public int OrderIndex { get; init; }

    public bool IsCorrect { get; init; }
}
