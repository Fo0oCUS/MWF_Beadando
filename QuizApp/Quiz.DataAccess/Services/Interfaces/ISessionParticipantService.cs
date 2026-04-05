using Quiz.DataAccess.Models;

namespace Quiz.DataAccess.Services.Interfaces;

public interface ISessionParticipantService
{
    Task<SessionParticipant> JoinByCodeAsync(string joinCode, string nickname, string? userId);

    Task<SessionParticipant> GetByIdAsync(int participantId);
}
