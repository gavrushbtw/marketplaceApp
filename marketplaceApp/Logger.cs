using System;
using System.IO;

public static class Logger
{
    private static string logPath = "security.log";

    public static void Log(string message)
    {
        string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}";
        File.AppendAllText(logPath, logMessage + Environment.NewLine);
    }
}