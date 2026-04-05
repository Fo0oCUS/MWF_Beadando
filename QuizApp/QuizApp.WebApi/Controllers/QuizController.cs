using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz.DataAccess.Services.Interfaces;
using Shared.Models.Request;
using Shared.Models.Responses;

namespace QuizApp.WebApi.Controllers;

[ApiController]
[Route("/quizzes")]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public QuizController(IQuizService quizService, IMapper mapper, IUserService userService)
    {
        _quizService = quizService;
        _mapper = mapper;
        _userService = userService;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(statusCode: StatusCodes.Status201Created, Type = typeof(QuizDetailsResponseDto))]
    public async Task<IActionResult> CreateQuiz([FromBody] QuizRequestDto quizRequestDto)
    {
        var quiz = _mapper.Map<Quiz.DataAccess.Models.Quiz>(quizRequestDto);
        var user = await _userService.GetCurrentUserAsync();
        quiz.CreatedByUserId = user!.Id;
        
        var createdQuiz = await _quizService.AddAsync(quiz);

        var response = _mapper.Map<QuizDetailsResponseDto>(createdQuiz);

        return CreatedAtAction(nameof(GetQuizById), new { quizId = response.Id }, response);
    }
    
    [HttpGet("{quizId}")]
    [Authorize]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(QuizDetailsResponseDto))]
    public async Task<IActionResult> GetQuizById([FromRoute] int quizId)
    {
        var quiz = await _quizService.GetByIdAsync(quizId);

        var response = _mapper.Map<QuizDetailsResponseDto>(quiz);
        return Ok(response);
    }

    [HttpGet("mine")]
    [Authorize]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(List<QuizSummaryResponseDto>))]
    public async Task<IActionResult> GetMyQuizzes()
    {
        var userId = _userService.GetCurrentUserId() ?? throw new UnauthorizedAccessException("User is not authenticated.");
        var quizzes = await _quizService.GetByUserIdAsync(userId);

        var response = _mapper.Map<List<QuizSummaryResponseDto>>(quizzes);
        return Ok(response);
    }
}
