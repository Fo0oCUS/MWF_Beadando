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

    public async Task<QuizSession> CreateAsync(int quizId, string hostUserId)
    {
        if (_userService.GetCurrentUserId() != hostUserId)
        {
            throw new UnauthorizedAccessException("You have no right to create this quiz session.");
        }

        var quiz = await _context.Quizzes
            .Include(x => x.Questions)
            .FirstOrDefaultAsync(x => x.Id == quizId);

        if (quiz == null)
        {
            throw new EntityNotFoundException("Quiz not found with id " + quizId);
        }

        if (quiz.CreatedByUserId != hostUserId)
        {
            throw new UnauthorizedAccessException("You can only host your own quiz.");
        }

        if (!quiz.IsPublished)
        {
            throw new AccessViolationException("Only published quizzes can be started.");
        }

        var quizSession = new QuizSession
        {
            QuizId = quizId,
            HostUserId = hostUserId,
            JoinCode = await GenerateJoinCodeAsync(),
            QuizSessionStatus = Models.Enums.QuizSessionStatus.Waiting,
            CurrentQuestionIndex = -1,
        };

        await _context.QuizSessions.AddAsync(quizSession);
        try
        {
            await _context.SaveChangesAsync();
            return await GetByIdAsync(quizSession.Id);
        }
        catch (DbUpdateException ex)
        {
            throw new SaveFailedException("Failed to create quiz session.", ex);
        }
    }

    public async Task<QuizSession> GetByIdAsync(int sessionId)
    {
        var session = await LoadSessionQuery().FirstOrDefaultAsync(x => x.Id == sessionId);
        if (session == null)
        {
            throw new EntityNotFoundException("Quiz session not found with id " + sessionId);
        }

        EnsureHostCanAccess(session);
        return session;
    }

    public async Task<QuizSession> GetByJoinCodeAsync(string joinCode)
    {
        var session = await LoadSessionQuery().FirstOrDefaultAsync(x => x.JoinCode == joinCode);
        if (session == null)
        {
            throw new EntityNotFoundException("Quiz session not found with join code " + joinCode);
        }

        return session;
    }

    public async Task<QuizSession?> GetLatestForQuizAsync(int quizId)
    {
        var quiz = await _context.Quizzes.FirstOrDefaultAsync(x => x.Id == quizId);
        if (quiz == null)
        {
            throw new EntityNotFoundException("Quiz not found with id " + quizId);
        }

        if (quiz.CreatedByUserId != _userService.GetCurrentUserId())
        {
            throw new UnauthorizedAccessException("You can only access your own quiz sessions.");
        }

        return await LoadSessionQuery()
            .Where(x => x.QuizId == quizId)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<QuizSession> StartAsync(int sessionId)
    {
        var session = await GetByIdAsync(sessionId);
        var orderedQuestions = session.Quiz?.Questions.OrderBy(x => x.OrderIndex).ToList() ?? new List<Question>();
        if (orderedQuestions.Count == 0)
        {
            throw new AccessViolationException("Quiz does not contain any questions.");
        }

        session.StartedAt ??= DateTime.UtcNow;
        session.EndedAt = null;
        session.CurrentQuestionIndex = 0;
        session.QuizSessionStatus = Models.Enums.QuizSessionStatus.InProgress;

        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<QuizSession> CloseCurrentQuestionAsync(int sessionId)
    {
        var session = await GetByIdAsync(sessionId);
        if (session.QuizSessionStatus != Models.Enums.QuizSessionStatus.InProgress)
        {
            throw new AccessViolationException("There is no open question to close.");
        }

        session.QuizSessionStatus = Models.Enums.QuizSessionStatus.Waiting;
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<QuizSession> AdvanceToNextQuestionAsync(int sessionId)
    {
        var session = await GetByIdAsync(sessionId);
        var orderedQuestions = session.Quiz?.Questions.OrderBy(x => x.OrderIndex).ToList() ?? new List<Question>();
        if (orderedQuestions.Count == 0)
        {
            throw new AccessViolationException("Quiz does not contain any questions.");
        }

        var nextQuestionIndex = session.CurrentQuestionIndex + 1;
        if (nextQuestionIndex >= orderedQuestions.Count)
        {
            session.QuizSessionStatus = Models.Enums.QuizSessionStatus.Finished;
            session.EndedAt = DateTime.UtcNow;
        }
        else
        {
            session.CurrentQuestionIndex = nextQuestionIndex;
            session.QuizSessionStatus = Models.Enums.QuizSessionStatus.InProgress;
        }

        await _context.SaveChangesAsync();
        return session;
    }

    private IQueryable<QuizSession> LoadSessionQuery()
    {
        return _context.QuizSessions
            .Include(x => x.Quiz)
            .ThenInclude(x => x!.Questions.OrderBy(q => q.OrderIndex))
            .ThenInclude(x => x.AnswerOptions.OrderBy(a => a.OrderIndex))
            .Include(x => x.Participants)
            .ThenInclude(x => x.ParticipantAnswers);
    }

    private void EnsureHostCanAccess(QuizSession session)
    {
        if (_userService.GetCurrentUserId() != session.HostUserId)
        {
            throw new UnauthorizedAccessException("You have no right to access this quiz session.");
        }
    }

    private async Task<string> GenerateJoinCodeAsync()
    {
        const string characters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();

        while (true)
        {
            var joinCodeChars = Enumerable.Range(0, 6)
                .Select(_ => characters[random.Next(characters.Length)])
                .ToArray();
            var joinCode = new string(joinCodeChars);

            var exists = await _context.QuizSessions.AnyAsync(x => x.JoinCode == joinCode);
            if (!exists)
            {
                return joinCode;
            }
        }
    }
}
