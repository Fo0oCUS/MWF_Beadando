using Microsoft.EntityFrameworkCore;
using Quiz.DataAccess.Models;
using Quiz.DataAccess.Services.Expections;
using Quiz.DataAccess.Services.Interfaces;

namespace Quiz.DataAccess.Services;

public class SessionParticipantService : ISessionParticipantService
{
    private readonly QuizAppDbContext _context;
    private readonly IUserService _userService;
    
    public SessionParticipantService(QuizAppDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<SessionParticipant> AddAsync(SessionParticipant sessionParticipant)
    {
        if (!_userService.CurrentUserOrAdmin(sessionParticipant.UserId!))
        {
            throw new UnauthorizedAccessException("You have no right to create this session participant.");
        }

        QuizSession? quizSession =
            _context.QuizSessions.FirstOrDefault(x => x.Id == sessionParticipant.QuizSessionId, null);
        if (quizSession == null)
        {
            throw new EntityNotFoundException("Quiz session not found with id " + sessionParticipant.QuizSessionId);
        }
        
        AppUser? appUser =
            _context.AppUsers.FirstOrDefault(x => x.Id == sessionParticipant.UserId, null);
        if (appUser == null)
        {
            throw new EntityNotFoundException("User not found with id " + sessionParticipant.QuizSessionId);
        }

        await _context.SessionParticipants.AddAsync(sessionParticipant);
        try
        {
            await _context.SaveChangesAsync();
            return sessionParticipant;
        }
        catch (DbUpdateException ex)
        {
            throw new SaveFailedException("Failed to create quiz.", ex);
        }
    }
}