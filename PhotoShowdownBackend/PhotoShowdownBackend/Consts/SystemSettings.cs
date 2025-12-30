using System.Text.Json;

namespace PhotoShowdownBackend.Consts;

public static class SystemSettings
{
    // Folders
    public const string PicturesFolderName = "pictures";
    public const string SQLScriptsFolderName = "SQL";

    // Database
    public const string DatabaseName = "PhotoShowdownDB";

    // Matches
    public const int ROUND_WINNER_DISPLAY_SECONDS = 15;

    // Features
    public const string ENABLE_SKIP_WHEN_ALL_VOTED_KEY = "ENABLE_SKIP_WHEN_ALL_VOTED";

    public static JsonSerializerOptions JsonSerializerOptions = null!;
}
