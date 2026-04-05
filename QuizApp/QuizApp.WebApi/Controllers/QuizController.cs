

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
    [ProducesResponseType(statusCode:201, Type = typeof(QuizResponseDto))]
    public async Task<IActionResult> CreateQuiz([FromBody] QuizRequestDto quizRequestDto)
    {
        var quiz = _mapper.Map<Quiz.DataAccess.Models.Quiz>(quizRequestDto);
        var user = await _userService.GetCurrentUserAsync();
        quiz.CreatedByUserId = user!.Id;
        
        var createdQuiz = await _quizService.AddAsync(quiz);

        var response = _mapper.Map<QuizResponseDto>(createdQuiz);

        return CreatedAtAction(nameof(CreateQuiz), new { id = response.Id }, response);
    }
    
    [HttpGet]
    [Route("quizzes/{quizId}")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(QuizResponseDto))]
    public async Task<IActionResult> GetQuizById([FromRoute] int quizId)
    {
        var quiz = await _quizService.GetByIdAsync(quizId);

        var response = _mapper.Map<QuizResponseDto>(quiz);
        return Ok(response);
    }

    [HttpGet]
    [Route("/quizzes/{userId}")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(List<QuizResponseDto>))]
    public async Task<IActionResult> GetUsersQuizzes([FromRoute] string userId)
    {
        var quizzes = await _quizService.GetByUserIdAsync(userId);

        var response = _mapper.Map<List<QuizResponseDto>>(quizzes);
        return Ok(response);
    }
}