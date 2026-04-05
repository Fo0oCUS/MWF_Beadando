using System.ComponentModel.DataAnnotations;

namespace Quiz.DataAccess.Models;

public class SessionParticipant
{
    [Key]
    public int Id { get; set; }

    public int QuizSessionId { get; set; }
    public QuizSession? QuizSession { get; set; }

    public string? UserId { get; set; }
    public AppUser? User { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nickname { get; set; } = string.Empty;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public int TotalScore { get; set; } = 0;

    public bool IsConnected { get; set; } = true;

    public string? ConnectionId { get; set; }

    public bool IsHost { get; set; }

    public ICollection<ParticipantAnswer> ParticipantAnswers { get; set; } = new List<ParticipantAnswer>();
}