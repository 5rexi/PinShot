namespace PinShot.App.Services;

internal static class AppLog
{
    private static readonly string LogPath = Path.Combine(Path.GetTempPath(), "PinShot.log");

    public static void Write(string message)
    {
        try
        {
            File.AppendAllText(LogPath, $"{DateTimeOffset.Now:O} {message}{Environment.NewLine}");
        }
        catch
        {
            // Logging must never prevent the app from starting.
        }
    }

    public static void Write(Exception exception, string context)
    {
        Write($"{context}: {exception}");
    }
}
