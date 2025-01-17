﻿using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;
using System.Linq.Expressions;

namespace PhotoShowdownBackend.Repositories.Rounds;

public interface IRoundsRepository: IRepository<Round>
{
  Task<Round?> GetLastRoundByMatchIdWithIncludes(int matchId,bool tracked = true);
  Task<Round?> GetWithIncludesAsync(Expression<Func<Round, bool>> filter,bool tracked = true);
}
