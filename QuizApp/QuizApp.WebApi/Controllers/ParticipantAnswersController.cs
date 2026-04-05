using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz.DataAccess.Models;
using Quiz.DataAccess.Services.Interfaces;
using Shared.Models.Request;

namespace QuizApp.WebApi.Controllers;

[ApiController]
[Route("/participant-answers")]
public class ParticipantAnswersController : ControllerBase
{
    private readonly IParticipantAnswerService _participantAnswerService;

    public ParticipantAnswersController(IParticipantAnswerService participantAnswerService)
    {
        _participantAnswerService = participantAnswerService;
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CreateAnswer([FromBody] SubmitParticipantAnswerRequestDto request)
    {
        var answer = new ParticipantAnswer
        {
            SessionParticipantId = request.SessionParticipantId,
            AnswerOptionId = request.AnswerOptionId,
            ResponseTimeMs = request.ResponseTimeMs,
        };

        await _participantAnswerService.AddAsync(answer);
        return NoContent();
    }
}
