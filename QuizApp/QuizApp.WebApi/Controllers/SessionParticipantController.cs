using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz.DataAccess.Models;
using Quiz.DataAccess.Services.Interfaces;
using Shared.Models.Request;
using Shared.Models.Responses;

namespace QuizApp.WebApi.Controllers;

[ApiController]
[Route("/sessionParticipant")]
public class SessionParticipantController : ControllerBase
{
    private readonly ISessionParticipantService _sessionParticipantService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public SessionParticipantController(ISessionParticipantService sessionParticipantService, IMapper mapper, IUserService userService)
    {
        _sessionParticipantService = sessionParticipantService;
        _mapper = mapper;
        _userService = userService;
    }
    
    [HttpPost]
    [ProducesResponseType(type: typeof(SessionParticipantResponseDto), statusCode: StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSessionParticipant(
        [FromBody] SessionParticipantRequestDto sessionParticipantRequestDto)
    {
        SessionParticipant sessionParticipant = _mapper.Map<SessionParticipant>(sessionParticipantRequestDto);
        var userId = _userService.GetCurrentUserId();
        sessionParticipant.UserId = userId;

        await _sessionParticipantService.AddAsync(sessionParticipant);
        var response = _mapper.Map<SessionParticipantResponseDto>(sessionParticipant);
        
        return CreatedAtAction(nameof(CreateSessionParticipant), new {Id = response.Id}, response);
    }
}