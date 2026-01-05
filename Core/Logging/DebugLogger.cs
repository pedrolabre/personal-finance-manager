using System;
using System.IO;

namespace PersonalFinanceManager.Core.Logging;

public static class DebugLogger
{
    private static readonly string LogPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PersonalFinanceManager",
        "debug.log"
    );

    static DebugLogger()
    {
        try
        {
            var dir = Path.GetDirectoryName(LogPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir!);
            }
            
            // Limpar log antigo ao iniciar
            if (File.Exists(LogPath))
            {
                File.Delete(LogPath);
            }
        }
        catch
        {
            // Ignorar erros na inicialização
        }
    }

    public static void Log(string message)
    {
        try
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var logMessage = $"[{timestamp}] {message}";
            
            System.Diagnostics.Debug.WriteLine(logMessage);
            File.AppendAllText(LogPath, logMessage + Environment.NewLine);
        }
        catch
        {
            // Ignorar erros de log
        }
    }

    public static string GetLogPath() => LogPath;
}
