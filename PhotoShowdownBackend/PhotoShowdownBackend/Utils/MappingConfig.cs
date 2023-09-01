using AutoMapper;
using PhotoShowdownBackend.Dtos.PicturesDto;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Models;

namespace PhotoShowdownBackend.Utils;

/// <summary>
/// Configuration class for AutoMapper.
/// Used to map between DTOs and Models
/// </summary>
public class MappingConfig: Profile
{
    public MappingConfig()
    {
        // Users
        CreateMap<RegisterationRequestDTO, User>();
        CreateMap<UserDTO, User>().ReverseMap();

        // Pictures
        CreateMap<Picture, PictureDTO>();
    }
}
