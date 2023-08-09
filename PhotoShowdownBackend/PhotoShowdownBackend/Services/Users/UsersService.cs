using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using PhotoShowdownBackend.Consts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PhotoShowdownBackend.Services.Users;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;

    private const int TOKEN_EXPIRATION_HOURS = 5;

    public UsersService(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }
    public async Task<RegisterationResponseDTO> RegisterUser(RegisterationRequestDTO registerationRequest)
    {
        var isUniqueUser = await _usersRepository.IsUniqueUser(registerationRequest.Username, registerationRequest.Email);
        if (!isUniqueUser)
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
        var user = await _usersRepository.GetAsync(u => u.Username == loginRequest.Username);
        if (user == null)
        {
            throw new InvalidLoginException();
        }
        if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
        {
            throw new InvalidLoginException();
        }
        var response = new LoginResponseDTO
        {
            Token = CreateToken(user)
        };
        return response;
    }

    private static string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(UserClaims.Id, user.Id.ToString()),
            new Claim(UserClaims.Username, user.Username),
            new Claim(UserClaims.Roles,"User"),
            new Claim(UserClaims.Roles,"Donfil"),
        };

        // TODO: define a actual key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("All Of the sudden donfil met"));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
                       issuer: "PhotoShowdown",
                       audience: "PhotoShowdown",
                       claims: claims,
                       expires: DateTime.Now.AddHours(TOKEN_EXPIRATION_HOURS),
                       signingCredentials: creds
                       );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}
