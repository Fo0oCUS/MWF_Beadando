using Microsoft.EntityFrameworkCore;
using Quiz.DataAccess.Models;
using Quiz.DataAccess.Services.Expections;
using Quiz.DataAccess.Services.Interfaces;

namespace Quiz.DataAccess.Services;

public class ParticipantAnswerService : IParticipantAnswerService
{
    private readonly QuizAppDbContext _context;

    public ParticipantAnswerService(QuizAppDbContext context)
    {
        _context = context;
    }

    public async Task<ParticipantAnswer> AddAsync(ParticipantAnswer participantAnswer)
    {
        SessionParticipant? sessionParticipant = await _context.SessionParticipants
            .Include(x => x.QuizSession)
            .FirstOrDefaultAsync(x => x.Id == participantAnswer.SessionParticipantId);
        if (sessionParticipant == null)
        {
            throw new EntityNotFoundException("Session participant not found with id " + participantAnswer.SessionParticipantId);
        }

        var quizSession = sessionParticipant.QuizSession ?? throw new EntityNotFoundException("Quiz session not found.");
        if (quizSession.QuizSessionStatus != Models.Enums.QuizSessionStatus.InProgress)
        {
            throw new AccessViolationException("Question is not open for answers.");
        }

        var currentQuestion = await _context.Questions
            .Include(x => x.AnswerOptions)
            .Where(x => x.QuizId == quizSession.QuizId)
            .OrderBy(x => x.OrderIndex)
            .Skip(quizSession.CurrentQuestionIndex)
            .FirstOrDefaultAsync();

        if (currentQuestion == null)
        {
            throw new EntityNotFoundException("Current question not found.");
        }

        var existingAnswer = await _context.ParticipantAnswers
            .FirstOrDefaultAsync(x =>
                x.SessionParticipantId == participantAnswer.SessionParticipantId &&
                x.QuestionId == currentQuestion.Id);

        if (existingAnswer != null)
        {
            throw new AccessViolationException("Participant has already answered this question.");
        }

        var answerOption = currentQuestion.AnswerOptions.FirstOrDefault(x => x.Id == participantAnswer.AnswerOptionId);
        if (answerOption == null)
        {
            throw new EntityNotFoundException("Answer option not found for the current question.");
        }

        participantAnswer.QuestionId = currentQuestion.Id;
        participantAnswer.IsCorrect = answerOption.IsCorrect;
        participantAnswer.AwardedPoints = answerOption.IsCorrect ? 100 : 0;
        participantAnswer.AnsweredAt = DateTime.UtcNow;

        await _context.ParticipantAnswers.AddAsync(participantAnswer);
        sessionParticipant.TotalScore += participantAnswer.AwardedPoints;

        try
        {
            await _context.SaveChangesAsync();
            return participantAnswer;
        }
        catch (DbUpdateException ex)
        {
            throw new SaveFailedException("Failed to create quiz.", ex);
        }
    }

    public Task<ParticipantAnswer?> GetForParticipantAndQuestionAsync(int sessionParticipantId, int questionId)
    {
        return _context.ParticipantAnswers.FirstOrDefaultAsync(x =>
            x.SessionParticipantId == sessionParticipantId &&
            x.QuestionId == questionId);
    }
}
