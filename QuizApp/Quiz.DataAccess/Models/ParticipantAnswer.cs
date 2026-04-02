using System.ComponentModel.DataAnnotations;

namespace Quiz.DataAccess.Models;

public class ParticipantAnswer
{
    [Key]
    public int Id { get; set; }

    public int SessionParticipantId { get; set; }
    public SessionParticipant? SessionParticipant { get; set; }

    public int QuestionId { get; set; }
    public Question? Question { get; set; }

    public int AnswerOptionId { get; set; }
    public AnswerOption? AnswerOption { get; set; }

    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

    public bool IsCorrect { get; set; }

    public int AwardedPoints { get; set; }

    public int ResponseTimeMs { get; set; }
}