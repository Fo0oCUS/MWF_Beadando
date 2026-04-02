using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Request;

public class QuizRequestDto
{
    [MaxLength(50, ErrorMessage = "Quiz title is too long. Maximum 50 characters.")]
    [MinLength(3, ErrorMessage = "Quiz title is too short. Minimum 3 characters.")]
    public required string Title { get; init; }

    public required string CreatedByUserId { get; init; }

    public bool IsPublished { get; init; }
}