using Microsoft.EntityFrameworkCore;
using Quiz.DataAccess.Models;
using Quiz.DataAccess.Services.Expections;

namespace Quiz.DataAccess.Services;

public class AnswerOptionService
{
    private readonly QuizAppDbContext _context;

    public AnswerOptionService(QuizAppDbContext context)
    {
        _context = context;
    }

    public async Task<AnswerOption> AddAsync(AnswerOption answerOption)
    {
        var questionId = answerOption.QuestionId;
        if (await _context.Questions.FirstOrDefaultAsync(x => x.Id == questionId) == null)
        {
            throw new EntityNotFoundException("Question doesn't exists with id " + questionId);
        }

        await _context.AnswerOptions.AddAsync(answerOption);
        try
        {
            await _context.SaveChangesAsync();
            return answerOption;
        }
        catch (DbUpdateException ex)
        {
            throw new SaveFailedException("Failed to create answer option.", ex);
        }
    }
}