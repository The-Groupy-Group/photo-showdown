using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;
using PhotoShowdownBackend.Repositories.Rounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoShowdownBackend.Tests.Repositories;

public class RoundsRepositoryTests
{
    // ------------------- CreateAsync ------------------- //
    [Fact]
    public async Task CreateAsync_Returns_A_Created_Round()
    {
        // Arrange
        var _db = TestUtils.GetInMemoryContext();
        var matchOwner = new User
        {
            Username = "test",
            Email = "",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test")
        };
        var newMatch = new Match
        {
            OwnerId = matchOwner.Id,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1)
        };
        _db.Users.Add(matchOwner);
        _db.Matches.Add(newMatch);
        IRoundsRepository repo = new RoundsRepository(_db);

        // Act
        var round = new Round
        {
            MatchId = newMatch.Id,
            RoundIndex = 1,
            RoundState = Round.RoundStates.NotStarted
        };
        var roundCreated = await repo.CreateAsync(round);

        // Assert
        Assert.Equal(round.MatchId, roundCreated.MatchId);
        Assert.Equal(round.RoundIndex, roundCreated.RoundIndex);
    }
}
