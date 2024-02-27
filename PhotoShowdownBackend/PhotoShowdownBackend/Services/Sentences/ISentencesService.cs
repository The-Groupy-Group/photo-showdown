namespace PhotoShowdownBackend.Services.CustomSentences;

public interface ISentencesService
{
     Task<string?> FetchSentence(int matchId);
     Task SetCustomSentences(List<string> sentenes, int matchId);
}
