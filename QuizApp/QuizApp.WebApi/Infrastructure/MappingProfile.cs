using AutoMapper;
using Quiz.DataAccess.Models;
using Shared.Models.Request;
using Shared.Models.Responses;

namespace QuizApp.WebApi.Infrastructure;

public class MappingProfile : Profile
{
    
    public MappingProfile()
    {
        // REQUEST DTO -> ENTITY

        CreateMap<AppUserRequestDto, AppUser>(MemberList.Source)
            .ForSourceMember(src => src.Password, opt => opt.DoNotValidate())
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(x=>x.Name));

        CreateMap<QuizRequestDto, Quiz.DataAccess.Models.Quiz>(MemberList.Source);

        CreateMap<QuestionRequestDto, Question>(MemberList.Source);

        CreateMap<AnswerOptionRequestDto, AnswerOption>(MemberList.Source);

        CreateMap<QuizSessionRequestDto, QuizSession>(MemberList.Source);

        CreateMap<SessionParticipantRequestDto, SessionParticipant>(MemberList.Source);

        CreateMap<ParticipantAnswerRequestDto, ParticipantAnswer>(MemberList.Source);

        
        // ENTITY -> RESPONSE DTO

        CreateMap<AppUser, AppUserResponseDto>(MemberList.Destination);

        CreateMap<Quiz.DataAccess.Models.Quiz, QuizResponseDto>(MemberList.Destination);

        CreateMap<Question, QuestionResponseDto>(MemberList.Destination);

        CreateMap<AnswerOption, AnswerOptionResponseDto>(MemberList.Destination);

        CreateMap<QuizSession, QuizSessionResponseDto>(MemberList.Destination);

        CreateMap<SessionParticipant, SessionParticipantResponseDto>(MemberList.Destination);

        CreateMap<ParticipantAnswer, ParticipantAnswerResponseDto>(MemberList.Destination);

        CreateMap<Quiz.DataAccess.Models.Quiz, QuizSummaryResponseDto>(MemberList.Destination)
            .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.Questions.Count));

        CreateMap<Quiz.DataAccess.Models.Quiz, QuizDetailsResponseDto>(MemberList.Destination);
        CreateMap<Question, QuizQuestionDetailsResponseDto>(MemberList.Destination);
        CreateMap<AnswerOption, QuizAnswerOptionDetailsResponseDto>(MemberList.Destination);

        CreateMap<SessionParticipant, SessionJoinResponseDto>(MemberList.Destination)
            .ForMember(dest => dest.ParticipantId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.QuizSessionId))
            .ForMember(dest => dest.JoinCode, opt => opt.MapFrom(src => src.QuizSession != null ? src.QuizSession.JoinCode : string.Empty));

        CreateMap<AnswerOption, SessionAnswerOptionViewResponseDto>(MemberList.Destination);
        CreateMap<Question, SessionQuestionViewResponseDto>(MemberList.Destination);
        CreateMap<AnswerOption, SessionAnswerResultItemResponseDto>(MemberList.Destination)
            .ForMember(dest => dest.AnswerOptionId, opt => opt.MapFrom(src => src.Id));
    }
}
