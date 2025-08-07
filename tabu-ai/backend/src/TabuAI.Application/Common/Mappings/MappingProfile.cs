using AutoMapper;
using TabuAI.Application.Common.DTOs;
using TabuAI.Application.Features.Game.Commands;
using TabuAI.Domain.Entities;

namespace TabuAI.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Word, WordDto>()
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => (int)src.Difficulty));

        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level.ToString()));

        CreateMap<GameSession, GameSessionDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.PromptQuality, opt => opt.MapFrom(src => src.PromptQuality.HasValue ? (int)src.PromptQuality.Value : (int?)null));

        CreateMap<StartGameRequest, StartGameCommand>();
        CreateMap<SubmitPromptRequest, SubmitPromptCommand>();
    }
}