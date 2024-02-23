using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;

namespace PhotoShowdownBackend.Repositories.Rounds;

public interface IRoundsRepository: IRepository<Round>
{
  Task<Round?> GetLast(int matchId,bool tracked = true);
}
