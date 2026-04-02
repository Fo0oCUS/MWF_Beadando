namespace Shared.Models.Responses;

public class QuizResponseDto
{
    public int Id { get; init; }

    public required string Title { get; init; }

    public required AppUserResponseDto CreatedByUser { get; init; }

    public bool IsPublished { get;  init; }

    public required ICollection<QuestionResponseDto> Questions { get; init; }
}