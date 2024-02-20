using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;
using PhotoShowdownBackend.Repositories.RoundPictures;
using PhotoShowdownBackend.Repositories.Rounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoShowdownBackend.Tests.Repositories;

public class RoundPicturesRepositoryTests
{
    // ------------------- CreateAsync ------------------- //
    [Fact]
    public async Task CreateAsync_Returns_A_Created_RoundPicture()
    {
        // Arrange
        var _db = TestUtils.GetInMemoryContext();
        var user = new User
        {
            Username = "test",
            Email = "",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test")
        };
        var picture = new Picture
        {
            User = user,
            PicturePath = "test"
        };
        var match = new Match
        {
            Owner = user,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1)
        };
        var round = new Round
        {
            Match = match,
            RoundIndex = 0
        };
        _db.Matches.Add(match);

        IRoundPicturesRepository repo = new RoundPicturesRepository(_db);

        // Act
        var roundPicture = new RoundPicture
        {
            Round = round,
            Picture = picture,
            User = user
        };
        var roundPictureCreated = await repo.CreateAsync(roundPicture);

        // Assert
        Assert.Equal(roundPicture.MatchId, roundPictureCreated.MatchId);
        Assert.Equal(roundPicture.RoundIndex, roundPictureCreated.RoundIndex);
        Assert.Equal(roundPicture.UserId, roundPictureCreated.UserId);
        Assert.Equal(roundPicture.PictureId, roundPictureCreated.PictureId);
    }
}
