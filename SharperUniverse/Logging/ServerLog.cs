using Serilog;

namespace SharperUniverse.Logging
{
    public static class ServerLog
    {
        static ServerLog()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
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