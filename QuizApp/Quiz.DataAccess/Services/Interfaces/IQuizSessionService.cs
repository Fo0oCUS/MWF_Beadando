using Quiz.DataAccess.Models;

namespace Quiz.DataAccess.Services.Interfaces;

public interface IQuizSessionService
{
    public Task<QuizSession> AddAsync(QuizSession quizSession);
}