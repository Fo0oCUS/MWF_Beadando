using Microsoft.EntityFrameworkCore;
using Quiz.DataAccess.Models;
using Quiz.DataAccess.Services.Expections;

namespace Quiz.DataAccess.Services.Interfaces;

public class ParticipantAnswerService
{
    private readonly QuizAppDbContext _context;
    private readonly IUserService _userService;
    
    public ParticipantAnswerService(QuizAppDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<ParticipantAnswer> AddAsync(ParticipantAnswer participantAnswer)
    {
        SessionParticipant? sessionParticipant =
            _context.SessionParticipants.FirstOrDefault(x => x.Id == participantAnswer.SessionParticipantId, null);
        if (sessionParticipant == null)
        {
            throw new EntityNotFoundException("Session participant not found with id " + participantAnswer.SessionParticipantId);
        }

        await _context.ParticipantAnswers.AddAsync(participantAnswer);
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
}