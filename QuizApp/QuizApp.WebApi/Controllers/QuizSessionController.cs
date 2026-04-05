using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz.DataAccess.Models;
using Quiz.DataAccess.Models.Enums;
using Quiz.DataAccess.Services.Interfaces;
using Shared.Models.Request;
using Shared.Models.Responses;

namespace QuizApp.WebApi.Controllers;

[ApiController]
[Route("/quiz-sessions")]
public class QuizSessionController : ControllerBase
{
    private readonly IQuizSessionService _quizSessionService;
    private readonly ISessionParticipantService _sessionParticipantService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public QuizSessionController(
        IQuizSessionService quizSessionService,
        ISessionParticipantService sessionParticipantService,
        IUserService userService,
        IMapper mapper)
    {
        _quizSessionService = quizSessionService;
        _sessionParticipantService = sessionParticipantService;
        _userService = userService;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(type: typeof(SessionStateResponseDto), statusCode: StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateQuizSession([FromBody] CreateQuizSessionRequestDto request)
    {
        var hostUserId = _userService.GetCurrentUserId() ?? throw new UnauthorizedAccessException("User is not authenticated.");
        var session = await _quizSessionService.CreateAsync(request.QuizId, hostUserId);
        var response = BuildSessionStateResponse(session, null);

        return CreatedAtAction(nameof(GetSessionById), new { sessionId = session.Id }, response);
    }

    [HttpGet("{sessionId}")]
    [Authorize]
    [ProducesResponseType(type: typeof(SessionStateResponseDto), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSessionById([FromRoute] int sessionId)
    {
        var session = await _quizSessionService.GetByIdAsync(sessionId);
        var response = BuildSessionStateResponse(session, null);
        return Ok(response);
    }

    [HttpGet("by-code/{joinCode}")]
    [AllowAnonymous]
    [ProducesResponseType(type: typeof(SessionStateResponseDto), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSessionByJoinCode([FromRoute] string joinCode, [FromQuery] int? participantId)
    {
        var session = await _quizSessionService.GetByJoinCodeAsync(joinCode);
        SessionParticipant? viewer = null;

        if (participantId.HasValue)
        {
            viewer = session.Participants.FirstOrDefault(x => x.Id == participantId.Value);
            if (viewer == null)
            {
                var fallbackViewer = await _sessionParticipantService.GetByIdAsync(participantId.Value);
                if (fallbackViewer.QuizSessionId != session.Id)
                {
                    throw new UnauthorizedAccessException("Participant does not belong to this session.");
                }

                viewer = session.Participants.FirstOrDefault(x => x.Id == fallbackViewer.Id);
            }
        }

        var response = BuildSessionStateResponse(session, viewer);

        return Ok(response);
    }

    [HttpGet("quiz/{quizId}/latest")]
    [Authorize]
    [ProducesResponseType(type: typeof(SessionStateResponseDto), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLatestSessionForQuiz([FromRoute] int quizId)
    {
        var session = await _quizSessionService.GetLatestForQuizAsync(quizId);
        if (session == null)
        {
            return NotFound();
        }

        var response = BuildSessionStateResponse(session, null);
        return Ok(response);
    }

    [HttpPatch("{sessionId}/start")]
    [Authorize]
    [ProducesResponseType(type: typeof(SessionStateResponseDto), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> StartSession([FromRoute] int sessionId)
    {
        var session = await _quizSessionService.StartAsync(sessionId);
        var response = BuildSessionStateResponse(session, null);
        return Ok(response);
    }

    [HttpPatch("{sessionId}/close-question")]
    [Authorize]
    [ProducesResponseType(type: typeof(SessionStateResponseDto), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> CloseCurrentQuestion([FromRoute] int sessionId)
    {
        var session = await _quizSessionService.CloseCurrentQuestionAsync(sessionId);
        var response = BuildSessionStateResponse(session, null);
        return Ok(response);
    }

    [HttpPatch("{sessionId}/next-question")]
    [Authorize]
    [ProducesResponseType(type: typeof(SessionStateResponseDto), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> AdvanceToNextQuestion([FromRoute] int sessionId)
    {
        var session = await _quizSessionService.AdvanceToNextQuestionAsync(sessionId);
        var response = BuildSessionStateResponse(session, null);
        return Ok(response);
    }

    private SessionStateResponseDto BuildSessionStateResponse(QuizSession session, SessionParticipant? viewer)
    {
        var orderedQuestions = session.Quiz?.Questions.OrderBy(question => question.OrderIndex).ToList() ?? new List<Question>();
        var currentQuestion = session.CurrentQuestionIndex >= 0 && session.CurrentQuestionIndex < orderedQuestions.Count
            ? orderedQuestions[session.CurrentQuestionIndex]
            : null;
        var currentAnswer = viewer != null && currentQuestion != null
            ? viewer.ParticipantAnswers.FirstOrDefault(answer => answer.QuestionId == currentQuestion.Id)
            : null;

        return new SessionStateResponseDto
        {
            SessionId = session.Id,
            QuizId = session.QuizId,
            QuizTitle = session.Quiz?.Title ?? string.Empty,
            JoinCode = session.JoinCode,
            Stage = GetStage(session),
            CurrentQuestionIndex = session.CurrentQuestionIndex,
            TotalQuestionCount = orderedQuestions.Count,
            ParticipantCount = session.Participants.Count,
            CanJoin = session.QuizSessionStatus != QuizSessionStatus.Finished,
            CanAnswer = session.QuizSessionStatus == QuizSessionStatus.InProgress &&
                        viewer != null &&
                        currentQuestion != null &&
                        currentAnswer == null,
            HasStarted = session.StartedAt != null,
            IsFinished = session.QuizSessionStatus == QuizSessionStatus.Finished,
            Viewer = viewer == null
                ? null
                : new ParticipantSessionViewResponseDto
                {
                    ParticipantId = viewer.Id,
                    Nickname = viewer.Nickname,
                    TotalScore = viewer.TotalScore,
                    HasAnsweredCurrentQuestion = currentAnswer != null,
                    SelectedAnswerOptionId = currentAnswer?.AnswerOptionId,
                },
            CurrentQuestion = currentQuestion == null
                ? null
                : _mapper.Map<SessionQuestionViewResponseDto>(currentQuestion),
            CurrentQuestionResults = currentQuestion == null || session.QuizSessionStatus == QuizSessionStatus.InProgress
                ? null
                : new SessionQuestionResultsResponseDto
                {
                    QuestionId = currentQuestion.Id,
                    Answers = currentQuestion.AnswerOptions
                        .OrderBy(answer => answer.OrderIndex)
                        .Select(answer => _mapper.Map<SessionAnswerResultItemResponseDto>(answer))
                        .ToList(),
                },
        };
    }

    private static string GetStage(QuizSession session)
    {
        if (session.QuizSessionStatus == QuizSessionStatus.Finished)
        {
            return "finished";
        }

        if (session.StartedAt == null || session.CurrentQuestionIndex < 0)
        {
            return "lobby";
        }

        return session.QuizSessionStatus == QuizSessionStatus.InProgress
            ? "question-open"
            : "question-closed";
    }
}
