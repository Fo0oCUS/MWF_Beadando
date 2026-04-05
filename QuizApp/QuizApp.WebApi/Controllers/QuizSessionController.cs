using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Quiz.DataAccess.Models;
using Quiz.DataAccess.Services.Expections;
using Quiz.DataAccess.Services.Interfaces;
using Shared.Models.Request;
using Shared.Models.Responses;

namespace QuizApp.WebApi.Controllers;

public class QuizSessionController : ControllerBase
{
    private readonly IQuizSessionService _quizSessionService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public QuizSessionController(IQuizSessionService quizSessionService, IMapper mapper, IUserService userService)
    {
        _quizSessionService = quizSessionService;
        _mapper = mapper;
        _userService = userService;
    }

    [HttpPost]
    [ProducesResponseType(type: typeof(QuizSessionResponseDto), statusCode: StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateQuizService([FromBody] QuizSessionRequestDto quizSessionRequestDto)
    {
        QuizSession quizSession = _mapper.Map<QuizSession>(quizSessionRequestDto);
        quizSession.HostUserId = _userService.GetCurrentUserId() ?? throw new UserCanNotBeNullException();

        await _quizSessionService.AddAsync(quizSession);
        var response = _mapper.Map<QuizSessionRequestDto>(quizSession);
        return CreatedAtAction(nameof(CreateQuizService), new {Id = quizSession.Id}, response);
    }
}