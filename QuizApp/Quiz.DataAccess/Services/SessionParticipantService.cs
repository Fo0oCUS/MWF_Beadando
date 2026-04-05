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

    public async Task<SessionParticipant> JoinByCodeAsync(string joinCode, string nickname, string? userId)
    {
        var normalizedNickname = nickname.Trim();
        if (string.IsNullOrWhiteSpace(normalizedNickname))
        {
            throw new ArgumentException("Nickname is required.");
        }

        if (userId != null && !_userService.CurrentUserOrAdmin(userId))
        {
            throw new UnauthorizedAccessException("You have no right to join with this user.");
        }

        QuizSession? quizSession = await _context.QuizSessions.FirstOrDefaultAsync(x => x.JoinCode == joinCode);
        if (quizSession == null)
        {
            throw new EntityNotFoundException("Quiz session not found with join code " + joinCode);
        }

        if (quizSession.QuizSessionStatus == Models.Enums.QuizSessionStatus.Finished)
        {
            throw new AccessViolationException("This quiz session is already finished.");
        }

        var existingParticipant = await _context.SessionParticipants
            .FirstOrDefaultAsync(x => x.QuizSessionId == quizSession.Id && x.Nickname == normalizedNickname);

        if (existingParticipant != null)
        {
            throw new InvalidOperationException("This nickname is already taken in the session.");
        }

        if (userId != null)
        {
            AppUser? appUser = await _context.AppUsers.FirstOrDefaultAsync(x => x.Id == userId);
            if (appUser == null)
            {
                throw new EntityNotFoundException("User not found with id " + userId);
            }
        }

        var sessionParticipant = new SessionParticipant
        {
            QuizSessionId = quizSession.Id,
            QuizSession = quizSession,
            UserId = userId,
            Nickname = normalizedNickname,
            IsConnected = true,
            JoinedAt = DateTime.UtcNow,
        };

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

    public async Task<SessionParticipant> GetByIdAsync(int participantId)
    {
        var participant = await _context.SessionParticipants.FirstOrDefaultAsync(x => x.Id == participantId);
        if (participant == null)
        {
            throw new EntityNotFoundException("Participant not found with id " + participantId);
        }

        return participant;
    }
}
