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
using PhotoShowdownBackend.Data;
using Microsoft.EntityFrameworkCore;
using PhotoShowdownBackend.Repositories.Repository;

namespace PhotoShowdownBackend.Tests.Repositories;

public class RepositoryTests
{
    // ------------------- CreateAsync ------------------- //
    [Fact]
    public async Task CreateAsync_Returns_A_Created_User()
    {
        // Arrange
        var _db = new PhotoShowdownDbContext(new DbContextOptionsBuilder<PhotoShowdownDbContext>().UseInMemoryDatabase("PhotoShowdown").Options);

        var repo = new Repository<User>(_db);
        // Act
        var newUser = await repo.CreateAsync(new User
        {
            Username = "test",
            Email = "",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test")
        });

        // Assert
        Assert.Equal("test", newUser.Username);
        Assert.True(_db.Users.Any(u => u.Id == newUser.Id && newUser.Username == u.Username));
    }
    // ------------------- RegisterUser ------------------- //
    [Fact]
    public async Task UpdateAsync_Returns_The_Updated_User()
    {
        // Arrange
        var _db = new PhotoShowdownDbContext(new DbContextOptionsBuilder<PhotoShowdownDbContext>().UseInMemoryDatabase("PhotoShowdown").Options);

        var newUser = new User
        {
            Username = "test",
            Email = "",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test")
        };

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();

        var repo = new Repository<User>(_db);

        User userToUpdate = (await repo.GetAsync(u => u.Id == newUser.Id))!;

        userToUpdate.Username = "test2";
        userToUpdate.Email = "test2@donfil.com";

        // Act
        var updatedUser = await repo.UpdateAsync(userToUpdate);

        // Assert
        Assert.Equal(userToUpdate.Username, updatedUser.Username);
        Assert.Equal(userToUpdate.Email, updatedUser.Email);
    }
    [Fact]
    public async Task UpdateAsync_Untracked_Throws_Already_Tracked()
    {
        // Arrange
        var _db = new PhotoShowdownDbContext(new DbContextOptionsBuilder<PhotoShowdownDbContext>().UseInMemoryDatabase("PhotoShowdown").Options);

        var newUser = new User
        {
            Username = "test",
            Email = "",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test")
        };

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();

        var repo = new Repository<User>(_db);

        User userToUpdate = (await repo.GetAsync(u => u.Id == newUser.Id, tracked: false))!;

        userToUpdate.Username = "test2";
        userToUpdate.Email = "test2@donfil.com";

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => repo.UpdateAsync(userToUpdate));
    }
    // ------------------- DeleteAsync ------------------- //
    [Fact]
    public async Task DeleteAsync_Returns_The_Deleted_User()
    {
        // Arrange
        var _db = new PhotoShowdownDbContext(new DbContextOptionsBuilder<PhotoShowdownDbContext>().UseInMemoryDatabase("PhotoShowdown").Options);

        var newUser = new User
        {
            Username = "test",
            Email = "",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test")
        };

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();

        var repo = new Repository<User>(_db);
        // Act
        var deletedUser = await repo.DeleteAsync(newUser);
  
        // Assert
        Assert.True(_db.Users.All(u => u.Id != deletedUser.Id));
    }
    // ------------------- GetAsync ------------------- //
    [Fact]
    public async Task GetAsync_Returns_The_User()
    {
        // Arrange
        var _db = new PhotoShowdownDbContext(new DbContextOptionsBuilder<PhotoShowdownDbContext>().UseInMemoryDatabase("PhotoShowdown").Options);

        var newUser = new User
        {
            Username = "test",
            Email = "",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test")
        };

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();

        var repo = new Repository<User>(_db);
        // Act
        var user = await repo.GetAsync(u=> u.Id == newUser.Id);

        // Assert
        Assert.Equal(user!.Username, newUser.Username);
        Assert.Equal(user.Email, newUser.Email);
    }
    // ------------------- GetAllAsync ------------------- //
    [Fact]
    public async Task GetAllAsync_Returns_The_Users()
    {
        // Arrange
        var _db = new PhotoShowdownDbContext(new DbContextOptionsBuilder<PhotoShowdownDbContext>().UseInMemoryDatabase("PhotoShowdown").Options);

        var newUser = new User
        {
            Username = "test",
            Email = "",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test")
        };

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();

        var repo = new Repository<User>(_db);
        // Act
        var users = await repo.GetAllAsync();

        // Assert
        var user = users[0];
        Assert.Equal(user!.Username, newUser.Username);
        Assert.Equal(user.Email, newUser.Email);
    }
}