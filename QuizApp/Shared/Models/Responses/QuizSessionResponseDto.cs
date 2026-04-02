using Quiz.DataAccess.Models.Enums;

namespace Shared.Models.Responses;

public class QuizSessionResponseDto
{
    public int Id { get; set; }

    public required QuizResponseDto Quiz { get;  set; }

    public required AppUserResponseDto HostUser { get; set; }
    
    public required string JoinCode { get; set; }

    public QuizSessionStatus QuizSessionStatus { get; set; }

    public int CurrentQuestionIndex { get; set; }
    
    public required ICollection<SessionParticipantResponseDto> Participants { get; set; }

    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
}