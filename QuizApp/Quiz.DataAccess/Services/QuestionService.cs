using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Quiz.DataAccess.Models;
using Quiz.DataAccess.Services.Expections;
using Quiz.DataAccess.Services.Interfaces;

namespace Quiz.DataAccess.Services;

public class QuestionService
{
    private readonly QuizAppDbContext _context;

    public QuestionService(QuizAppDbContext context)
    {
        _context = context;
    }

    public async Task<Question> AddAsync(Question question)
    {
        var quizId = question.QuizId;
        if (await _context.Quizzes.FirstOrDefaultAsync(x => x.Id == quizId) == null)
        {
            throw new EntityNotFoundException("Question doesn't exists with id " + quizId);
        }

        await _context.Questions.AddAsync(question);
        try
        {
            await _context.SaveChangesAsync();
            return question;
        }
        catch (DbUpdateException ex)
        {
            throw new SaveFailedException("Failed to create answer option.", ex);
        }
    }
}