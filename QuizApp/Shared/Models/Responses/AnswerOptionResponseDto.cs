namespace Shared.Models.Responses;

public class AnswerOptionResponseDto
{
    public int Id { get; init; }
    
    public required QuestionResponseDto Question { get; init; }

    public required string Text { get; init; }
    
    public int OrderIndex { get; init; }
    
    public bool IsCorrect { get; init; }
}