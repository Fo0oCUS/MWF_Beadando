using Quiz.DataAccess.Models;

namespace Quiz.DataAccess.Services.Interfaces;

public interface IParticipantAnswerService
{
    Task<ParticipantAnswer> AddAsync(ParticipantAnswer participantAnswer);

    Task<ParticipantAnswer?> GetForParticipantAndQuestionAsync(int sessionParticipantId, int questionId);
}
