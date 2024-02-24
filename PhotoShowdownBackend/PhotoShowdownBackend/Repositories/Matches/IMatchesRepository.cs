using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;
using System.Linq.Expressions;

namespace PhotoShowdownBackend.Repositories.Users;

/// <summary>
/// Data Access Layer (DAL) interface for our Users
/// </summary>
public interface IMatchesRepository : IRepository<Match>
{
    Task<List<Match>> GetAllWithUsersAsync(
        Expression<Func<Match, bool>>? filter = null, 
        bool tracked = true, 
        int? pageNumber = null, 
        int? pageSize = null);
    Task<Match?> GetWithUsersAsync(
        Expression<Func<Match, bool>> filter, 
        bool tracked = true);
}
