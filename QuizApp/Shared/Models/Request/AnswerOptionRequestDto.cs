using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Request;

public class AnswerOptionRequestDto
{

    [MinLength(1, ErrorMessage = "Answer is too short. Minimum 1 characters.")]
    [MaxLength(255, ErrorMessage = "Answer is too long. Maximum 255 characters.")]
    public required string Text { get; init; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Order index can not be less than 0.")]
    public int OrderIndex { get; init; }
    
    public bool IsCorrect { get; init; }
}