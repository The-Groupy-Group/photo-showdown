using Microsoft.AspNetCore.Mvc;
using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.Services.Users;

public interface IUsersService
{
    Task<RegisterationResponseDTO> RegisterUser(RegisterationRequestDTO registerationRequest);
    Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest);
}
