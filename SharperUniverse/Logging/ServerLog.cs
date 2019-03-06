using System;
using System.IO;
using Serilog;

namespace SharperUniverse.Logging
{
    public static class ServerLog
    {
        private static string LogFileLocation() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "SharperUniverse");

        static ServerLog()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.RollingFile(Path.Combine(LogFileLocation(), @"SharperUniverse-{HalfHour}.txt"))
                .CreateLogger();
        }

        public static void LogInfo(string message)
        {
            Log.Information(message);
        }

        public static void LogDebug(string message)
        {
            Log.Debug(message);
        }

        public static void LogWarning(string message)
        {
            Log.Warning(message);
        }

        public static void LogError(string message)
        {
            Log.Error(message);
        }
    }
}