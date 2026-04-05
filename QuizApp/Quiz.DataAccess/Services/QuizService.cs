using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Quiz.DataAccess.Services.Expections;
using Quiz.DataAccess.Services.Interfaces;

namespace Quiz.DataAccess.Services;

using Models;

public class QuizService : IQuizService
{
    private readonly QuizAppDbContext _context;
    private readonly IUserService _userService;
    
    public QuizService(QuizAppDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }


    public async Task<Quiz> GetByIdAsync(int id)
    {
        Quiz? quiz = await _context.Quizzes.FirstOrDefaultAsync(x => x.Id == id);
        if (quiz == null) throw new EntityNotFoundException(nameof(Quiz));
        
        var user = await _userService.GetUserByIdAsync(quiz.CreatedByUserId);

        if (!_userService.CurrentUserOrAdmin(user.Id))
        {
            throw new AccessViolationException("User not accessible");
        }
        
        return quiz;
    }

    public async Task<IReadOnlyCollection<Quiz>> GetByUserIdAsync(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (!_userService.CurrentUserOrAdmin(user.Id))
        {
            throw new AccessViolationException("User not accessible");
        }
        
        return await _context.Quizzes.Where(x => x.CreatedByUserId == id).ToListAsync();
    }
    
    public async Task<Quiz> AddAsync(Quiz quiz)
    {
        await _context.Quizzes.AddAsync(quiz);
        try
        {
            await _context.SaveChangesAsync();
            return quiz;
        }
        catch (DbUpdateException ex)
        {
            throw new SaveFailedException("Failed to create quiz.", ex);
        }
    }
}