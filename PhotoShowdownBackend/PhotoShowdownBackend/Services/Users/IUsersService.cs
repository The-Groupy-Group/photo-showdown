using Microsoft.AspNetCore.Mvc;
using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.Services.Users;

/// <summary>
/// A business logic service interface for our Users
/// </summary>
public interface IUsersService
{
    Task<RegisterationResponseDTO> CreateUser(RegisterationRequestDTO registerationRequest);
    Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest);
    Task<UserDTO> GetUser(int id);
    Task<UserPublicDetailsDTO> GetUserPublicDetails(int id);
    Task<bool> DoesUserExist(int userId);
}
