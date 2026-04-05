namespace Shared.Models.Responses;

public class SessionStateResponseDto
{
    public int SessionId { get; init; }

    public int QuizId { get; init; }

    public required string QuizTitle { get; init; }

    public required string JoinCode { get; init; }

    public required string Stage { get; init; }

    public int CurrentQuestionIndex { get; init; }

    public int TotalQuestionCount { get; init; }

    public int ParticipantCount { get; init; }

    public bool CanJoin { get; init; }

    public bool CanAnswer { get; init; }

    public bool HasStarted { get; init; }

    public bool IsFinished { get; init; }

    public ParticipantSessionViewResponseDto? Viewer { get; init; }

    public SessionQuestionViewResponseDto? CurrentQuestion { get; init; }

    public SessionQuestionResultsResponseDto? CurrentQuestionResults { get; init; }
}

public class ParticipantSessionViewResponseDto
{
    public int ParticipantId { get; init; }

    public required string Nickname { get; init; }

    public int TotalScore { get; init; }

    public bool HasAnsweredCurrentQuestion { get; init; }

    public int? SelectedAnswerOptionId { get; init; }
}

public class SessionQuestionViewResponseDto
{
    public int Id { get; init; }

    public required string Text { get; init; }

    public int OrderIndex { get; init; }

    public int TimeLimitSeconds { get; init; }

    public required ICollection<SessionAnswerOptionViewResponseDto> AnswerOptions { get; init; }
}

public class SessionAnswerOptionViewResponseDto
{
    public int Id { get; init; }

    public required string Text { get; init; }

    public int OrderIndex { get; init; }
}

public class SessionQuestionResultsResponseDto
{
    public int QuestionId { get; init; }

    public required ICollection<SessionAnswerResultItemResponseDto> Answers { get; init; }
}

public class SessionAnswerResultItemResponseDto
{
    public int AnswerOptionId { get; init; }

    public required string Text { get; init; }

    public bool IsCorrect { get; init; }
}
