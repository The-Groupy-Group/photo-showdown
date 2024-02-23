using AutoMapper;
using PhotoShowdownBackend.Consts;
using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Dtos.Pictures;
using PhotoShowdownBackend.Dtos.Rounds;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Extentions;
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
        CreateMap<RoundPicture,PictureSelectedDTO>()
            .ForMember(dest => dest.NumOfVotes, opt => opt.MapFrom(src => src.RoundVotes.Count))
            .ForMember(dest => dest.SelectedByUser, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.UsersVoted, opt => opt.MapFrom(src => src.RoundVotes.Select(rv => rv.User)));

        // Matches
        CreateMap<Match, MatchDTO>()
            .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.MatchConnections.Select(mc => mc.User)))
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
            .ForMember(dest => dest.MatchState, opt => opt.MapFrom(src => 
                DateTime.UtcNow > src.EndDate ? MatchStates.Ended : 
                DateTime.UtcNow > src.StartDate ? MatchStates.InProgress : 
                MatchStates.NotStarted));
        CreateMap<Match, MatchCreationResponseDTO>();

        // Rounds
        CreateMap<Round, RoundDTO>()
            .ForMember(dest => dest.PicturesSelected, opt => opt.MapFrom(src => src.RoundPictures))
            .ForMember(dest => dest.RoundWinner, opt => opt.MapFrom(src => src.Winner))
            .ForMember(dest => dest.PictureSelectionEndDate,
                opt => opt.MapFrom(src => src.StartDate!.Value.AddSeconds(
                    src.Match.PictureSelectionTimeSeconds)))
            .ForMember(dest => dest.VotingEndDate,
                opt => opt.MapFrom(src => src.StartDate!.Value.AddSeconds(
                    src.Match.PictureSelectionTimeSeconds).AddSeconds(
                    src.Match.VoteTimeSeconds)))
            .ForMember(dest => dest.RoundEndDate,
                opt => opt.MapFrom(src => src.StartDate!.Value.AddSeconds(
                    src.Match.PictureSelectionTimeSeconds).AddSeconds(
                    src.Match.VoteTimeSeconds).AddSeconds(
                    SystemSettings.ROUND_WINNER_DISPLAY_SECONDS)));

    }
}
