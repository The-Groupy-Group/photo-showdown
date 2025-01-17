using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoShowdownBackend.Controllers;
using PhotoShowdownBackend.Dtos;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Services.Session;
using PhotoShowdownBackend.Services.Users;

namespace PhotoShowdownBackend.Tests.Controllers;

public class UsersControllerTests
{
    // ------------------- Login ------------------- //
    [Fact]
    public async Task Login_Returns_OkObjectResult()
    {
        // Arrange
        var service = A.Fake<IUsersService>();
        var sessionService = A.Fake<ISessionService>();
        var logger = A.Fake<ILogger<UsersController>>();

        var loginReq = new LoginRequestDTO
        {
            Username = "test",
            Password = "test"
        };

        A.CallTo(() => service.Login(loginReq)).Returns(Task.FromResult<LoginResponseDTO>(new() { Token = "afakejwttoken" }));

        var controller = new UsersController(service, sessionService, logger);

        // Act
        var res = await controller.Login(loginReq);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(res);
        var apiRes = Assert.IsType<APIResponse<LoginResponseDTO>>(okResult.Value);
        Assert.True(apiRes.Data!.Token.Length > 0);
    }
    // ------------------- RegisterUser ------------------- //
    [Fact]
    public async Task RegisterUser_Returns_CreatedAtActionResult()
    {
        // Arrange
        var service = A.Fake<IUsersService>();
        var sessionService = A.Fake<ISessionService>();
        var logger = A.Fake<ILogger<UsersController>>();

        var regsiterReq = new RegisterationRequestDTO
        {
            Username = "test",
            Password = "test"
        };

        A.CallTo(() => service.CreateUser(regsiterReq)).Returns(Task.FromResult<RegisterationResponseDTO>(new() { Id = 1 }));

        var controller = new UsersController(service, sessionService, logger);

        // Act
        var res = await controller.Register(regsiterReq);

        // Assert
        var okResult = Assert.IsType<CreatedAtActionResult>(res);
        var apiRes = Assert.IsType<APIResponse<RegisterationResponseDTO>>(okResult.Value);
        Assert.True(apiRes.Data!.Id == 1);
    }
}
