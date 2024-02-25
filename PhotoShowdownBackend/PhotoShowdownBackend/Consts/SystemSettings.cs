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

    public static JsonSerializerOptions JsonSerializerOptions = null!;
}
