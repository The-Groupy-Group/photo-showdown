﻿using BCrypt.Net;

namespace PhotoShowdownBackend.Services.Users;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;

    public UsersService(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }
    public async Task<RegisterationResponseDTO> RegisterUser(RegisterationRequestDTO registerationRequest)
    {
        var isUniqueUser = await _usersRepository.IsUniqueUser(registerationRequest.Username, registerationRequest.Email);
        if(!isUniqueUser)
        {
            throw new UsersServiceException("Username or Email already exists");
        }
        var user = new User
        {
            Username = registerationRequest.Username,
            Email = registerationRequest.Email,
            FirstName = registerationRequest.FirstName,
            LastName = registerationRequest.LastName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerationRequest.Password)
        };

        var createdUser = await _usersRepository.CreateAsync(user);

        var response = new RegisterationResponseDTO
        {
            Id = createdUser.Id
        };

        return response;
    }
    public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest)
    {
        var user = await _usersRepository.GetAsync(u=>u.Username == loginRequest.Username);
        if(user == null)
        {
            throw new InvalidLoginException();
        }
        if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
        {
            throw new InvalidLoginException();
        }
        return new();
    }
}
