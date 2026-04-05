using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Quiz.DataAccess.Services.Interfaces;
using Shared.Models.Request;
using Shared.Models.Responses;

namespace QuizApp.WebApi.Controllers;

[ApiController]
[Route("/session-participants")]
public class SessionParticipantController : ControllerBase
{
    private readonly ISessionParticipantService _sessionParticipantService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public SessionParticipantController(ISessionParticipantService sessionParticipantService, IUserService userService, IMapper mapper)
    {
        _sessionParticipantService = sessionParticipantService;
        _userService = userService;
        _mapper = mapper;
    }

    [HttpPost("join")]
    [ProducesResponseType(type: typeof(SessionJoinResponseDto), statusCode: StatusCodes.Status201Created)]
    public async Task<IActionResult> JoinSession([FromBody] JoinSessionByCodeRequestDto request)
    {
        var participant = await _sessionParticipantService.JoinByCodeAsync(request.JoinCode, request.Nickname, _userService.GetCurrentUserId());
        var response = _mapper.Map<SessionJoinResponseDto>(participant);

        return StatusCode(StatusCodes.Status201Created, response);
    }
}
