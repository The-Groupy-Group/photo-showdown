using PhotoShowdownBackend.Data;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;

namespace PhotoShowdownBackend.Repositories.CustomSentences;

public class CustomSentencesRepository : Repository<CustomSentence>, ICustomSentencesRepository
{
    public CustomSentencesRepository(PhotoShowdownDbContext _db) : base(_db)
    {
        
    }
}
