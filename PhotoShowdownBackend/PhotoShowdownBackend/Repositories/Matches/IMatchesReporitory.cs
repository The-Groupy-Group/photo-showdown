﻿using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;
using System.Linq.Expressions;

namespace PhotoShowdownBackend.Repositories.Users;

/// <summary>
/// Data Access Layer (DAL) interface for our Users
/// </summary>
public interface IMatchesReporitory : IRepository<Match>
{
    Task<List<Match>> GetAllWithUsersAsync(Expression<Func<Match, bool>>? filter = null, bool tracked = true, int? pageNumber = null, int? pageSize = null);

}
