using Quiz.DataAccess.Models;

namespace Quiz.DataAccess.Services.Interfaces;

public interface IQuizSessionService
{
    Task<QuizSession> CreateAsync(int quizId, string hostUserId);

    Task<QuizSession> GetByIdAsync(int sessionId);

    Task<QuizSession> GetByJoinCodeAsync(string joinCode);

    Task<QuizSession?> GetLatestForQuizAsync(int quizId);

    Task<QuizSession> StartAsync(int sessionId);

    Task<QuizSession> CloseCurrentQuestionAsync(int sessionId);

    Task<QuizSession> AdvanceToNextQuestionAsync(int sessionId);
}
