using AutoMapper;
using Microsoft.Extensions.Configuration;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using PhotoShowdownBackend.Repositories.Users;
using PhotoShowdownBackend.Services.Users;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Utils;
using PhotoShowdownBackend.Models;
using System.Linq.Expressions;

namespace PhotoShowdownBackend.Tests.Services;

public class UsersServiceTests
{
    // ------------------- Login ------------------- //
    [Fact]
    public async Task Login_Returns_A_Valid_JWT()
    {
        // Arrange
        var userRepo = A.Fake<IUsersRepository>();
        var config = A.Fake<IConfiguration>();
        var configSec = A.Fake<IConfigurationSection>();
        var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(MappingConfig).Assembly)).CreateMapper();
        var logger = A.Fake<ILogger<UsersService>>();

        A.CallTo(() => configSec.Value).Returns("h74h346wh6sybserbyre34q");
        A.CallTo(() => config.GetSection("AppSettings:Token")).Returns(configSec);
        A.CallTo(() => userRepo.IsUniqueUser(A<string>.Ignored, A<string>.Ignored)).Returns(true);
        A.CallTo(() => userRepo.GetAsync(A<Expression<Func<User, bool>>>.Ignored, true)).Returns(Task.FromResult(new User
        {
            Id = 1,
            Username = "test",
            Email = "",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test")
        }) as Task<User?>);

        var service = new UsersService(userRepo, config, mapper, logger);

        // Act
        var res = await service.Login(new LoginRequestDTO
        {
            Username = "test",
            Password = "test"
        });

        // Assert
        Assert.NotNull(res.Token);
        Assert.True(res.Token.Length > 0);
    }
    // ------------------- RegisterUser ------------------- //
    [Fact]
    public async Task RegisterUser_Returns_A_Valid_Id()
    {
        // Arrange
        var userRepo = A.Fake<IUsersRepository>();
        var config = A.Fake<IConfiguration>();
        var configSec = A.Fake<IConfigurationSection>();
        var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(MappingConfig).Assembly)).CreateMapper();
        var logger = A.Fake<ILogger<UsersService>>();

        A.CallTo(() => userRepo.IsUniqueUser(A<string>.Ignored, A<string>.Ignored)).Returns(true);
        A.CallTo(() => userRepo.CreateAsync(A<User>.Ignored)).Returns(Task.FromResult(new User
        {
            Id = 1,
            Username = "test",
            Email = "",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test")
        }));

        var service = new UsersService(userRepo, config, mapper, logger);

        // Act
        var res = await service.RegisterUser(new RegisterationRequestDTO
        {
            Username = "test",
            Email = "",
            Password = "test"
        });

        // Assert
        Assert.True(res.Id == 1);
    }
}