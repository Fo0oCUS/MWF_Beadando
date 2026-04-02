using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Request;

public class QuestionRequestDto
{
    [MinLength(3, ErrorMessage = "Question is too short. Minimum 3 characters.")]
    [MaxLength(255, ErrorMessage = "Question is too long.")]
    public required string Text { get; init; }

    [Range(0, int.MaxValue, ErrorMessage = "Order index cannot be less than 0.")]
    public int OrderIndex { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Time limit should be greater than 1.")]
    public int TimeLimitSeconds { get; init; }

}