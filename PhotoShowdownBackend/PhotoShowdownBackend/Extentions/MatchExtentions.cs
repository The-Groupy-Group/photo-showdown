using PhotoShowdownBackend.Models;
using System.Net.WebSockets;

namespace PhotoShowdownBackend.Extentions;

public static class MatchExtentions
{
    public static bool HasMatchStarted(this Match match)
    {
        return match.StartDate != null && DateTime.UtcNow >= match.StartDate;
    }
}
