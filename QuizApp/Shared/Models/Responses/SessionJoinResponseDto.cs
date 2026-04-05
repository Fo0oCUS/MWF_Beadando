namespace Shared.Models.Responses;

public class SessionJoinResponseDto
{
    public int ParticipantId { get; init; }

    public int SessionId { get; init; }

    public required string JoinCode { get; init; }

    public required string Nickname { get; init; }

    public int TotalScore { get; init; }
}
