using Quiz.DataAccess.Models;

namespace Quiz.DataAccess.Services.Interfaces;

public interface ISessionParticipantService
{
    public Task<SessionParticipant> AddAsync(SessionParticipant sessionParticipant);
}