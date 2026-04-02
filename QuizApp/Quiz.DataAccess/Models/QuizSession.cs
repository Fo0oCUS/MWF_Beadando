using System.ComponentModel.DataAnnotations;
using Quiz.DataAccess.Models.Enums;

namespace Quiz.DataAccess.Models;

public class QuizSession
{
    [Key] public int Id { get; set; }

    public int QuizId { get; set; }
    public Quiz? Quiz { get;  set; }

    public int HostUserId { get; set; }
    public AppUser? HostUser { get; set; }
    
    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string JoinCode { get; set; } = "";

    public QuizSessionStatus QuizSessionStatus { get; set; } = QuizSessionStatus.Waiting;

    public int CurrentQuestionIndex { get; set; } = -1;
    
    public ICollection<SessionParticipant> Participants { get; set; } = new List<SessionParticipant>();

    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
}