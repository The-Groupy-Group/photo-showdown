using AutoMapper;
using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Dtos.PicturesDto;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Models;

namespace PhotoShowdownBackend.Utils;

/// <summary>
/// Configuration class for AutoMapper.
/// Used to map between DTOs and Models
/// </summary>
public class MappingConfig : Profile
{
    public MappingConfig()
    {
        // Users
        CreateMap<RegisterationRequestDTO, User>();
        CreateMap<UserDTO, User>().ReverseMap();
        CreateMap<UserPublicDetailsDTO, User>().ReverseMap();

        // Pictures
        CreateMap<Picture, PictureDTO>();

        // Matches
        CreateMap<Match, MatchDTO>()
            .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.MatchConnections.Select(mc => mc.User)))
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner));
        CreateMap<Match, MatchCreationResponseDTO>();
    }
}
