using System.ComponentModel.DataAnnotations;
using Quiz.DataAccess.Models.Enums;

namespace Shared.Models.Request;

public class QuizSessionRequestDto
{
    public int QuizId { get; init; }
    
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Join code must be 6 characters long.")]
    public required string JoinCode { get; init; }

    public QuizSessionStatus QuizSessionStatus { get; init; } = QuizSessionStatus.Waiting;

    public int CurrentQuestionIndex { get; init; } = -1;

    public DateTime? StartedAt { get; init; }
    public DateTime? EndedAt { get; init; }
}