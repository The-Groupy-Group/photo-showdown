using AutoMapper;
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
        CreateMap<RegisterationRequestDTO, User>();
    }
}
