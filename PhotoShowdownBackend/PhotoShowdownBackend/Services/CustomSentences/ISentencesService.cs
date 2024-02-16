namespace PhotoShowdownBackend.Services.CustomSentences
{
    public interface ISentencesService
    {
         Task<string> FetchSentence(int matchId);
    }
}
