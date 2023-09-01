using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;

namespace PhotoShowdownBackend.Repositories.Users;

/// <summary>
/// Data Access Layer (DAL) interface for our Users
/// </summary>
public interface IMatchesReporitory : IRepository<Match>
{
}
