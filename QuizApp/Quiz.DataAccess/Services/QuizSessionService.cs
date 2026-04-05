using Microsoft.EntityFrameworkCore;
using Quiz.DataAccess.Models;
using Quiz.DataAccess.Services.Expections;
using Quiz.DataAccess.Services.Interfaces;

namespace Quiz.DataAccess.Services;

public class QuizSessionService : IQuizSessionService
{
    private readonly QuizAppDbContext _context;
    private readonly IUserService _userService;
    
    public QuizSessionService(QuizAppDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<QuizSession> AddAsync(QuizSession quizSession)
    {
        if (_userService.GetCurrentUserId() != quizSession.HostUserId)
        {
            throw new UnauthorizedAccessException("You have no right to create this quiz session.");
        }

        await _context.QuizSessions.AddAsync(quizSession);
        try
        {
            await _context.SaveChangesAsync();
            return quizSession;
        }
        catch (DbUpdateException ex)
        {
            throw new SaveFailedException("Failed to create quiz session.", ex);
        }
    } 
}