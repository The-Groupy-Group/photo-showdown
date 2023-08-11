using AutoMapper;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using PhotoShowdownBackend.Consts;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Exceptions.Users;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PhotoShowdownBackend.Services.Users;

/// <summary>
/// A business logic service implementation for our Users
/// </summary>
public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ILogger<UsersService> _logger;

    private const int TOKEN_EXPIRATION_HOURS = 5;

    public UsersService(IUsersRepository usersRepository, IConfiguration configuration, IMapper mapper, ILogger<UsersService> logger)
    {
        _usersRepository = usersRepository;
        _configuration = configuration;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<RegisterationResponseDTO> RegisterUser(RegisterationRequestDTO registerationRequest)
    {
        // Verify that the username and email are unique
        var isUniqueUser = await _usersRepository.IsUniqueUser(registerationRequest.Username, registerationRequest.Email);
        if (!isUniqueUser)
        {
            throw new UsersServiceException("Username or Email already exists");
        }

        // Map the request to a User object
        var user = _mapper.Map<User>(registerationRequest);
        user.PasswordHash = HashPassword(registerationRequest.Password);

        // Create the user
        var createdUser = await _usersRepository.CreateAsync(user);

        // Create the response
        var response = new RegisterationResponseDTO
        {
            Id = createdUser.Id
        };

        return response;
    }

    public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest)
    {
        // Get the user by username
        var user = await _usersRepository.GetAsync(u => u.Username == loginRequest.Username);
        
        // Verify the user exists
        if (user == null)
        {
            throw new InvalidLoginException();
        }

        // Verify the user exists and the password is correct
        if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
        {
            throw new InvalidLoginException();
        }

        // Create the jwt token
        var jwtToken = CreateToken(user);

        var response = new LoginResponseDTO
        {
            Token = jwtToken
        };

        return response;
    }

    // Helpers
    private string CreateToken(User user)
    {
        // Create the claims that will be inserted into the token
        List<Claim> claims = new List<Claim>
        {
            new Claim(UserClaims.Id, user.Id.ToString()),
            new Claim(UserClaims.Username, user.Username),
            new Claim(UserClaims.Roles,Roles.User),
            new Claim(UserClaims.Roles,Roles.Donfil),
            new Claim(UserClaims.Roles,Roles.Admin),
        };

        // Get the secret signing key from the configuration
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));

        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        // Create the token
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
    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
