namespace AnimationBaked.Utils.Logger;

public static class Logger
{
    public static void LogInfo(string message)
    {
        Console.WriteLine($"[Info] {message}");
    }

    public static void LogError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {message}");
        Console.ResetColor();
    }

    public static void LogWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[WARNING] {message}");
        Console.ResetColor();
    }

    public static void LogPath(string message)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"[Path] {message}");
        Console.ResetColor();
    }
}