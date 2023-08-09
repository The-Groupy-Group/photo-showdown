using Microsoft.AspNetCore.Mvc;

namespace PhotoShowdownBackend.Services.Users;

public interface IUsersService
{
    Task<RegisterationResponseDTO> RegisterUser(RegisterationRequestDTO registerationRequest);
    Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest);
}
