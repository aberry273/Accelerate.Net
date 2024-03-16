using Microsoft.Extensions.Logging;

namespace Accelerate.Foundations.Common.Services
{
    public static class StaticLoggingService
    {
        private static ILoggerFactory _Factory = null;
        private static ILogger _logger { get; set; }

        public static ILogger Logger
        {
            get
            {
                if (_logger == null) ConfigureLogger(LoggerFactory);
                return _logger;
            }
        }

        public static void ConfigureLogger(ILoggerFactory factory)
        {
            //factory.CreateLogger();
            _logger = CreateLogger();


            //factory("Logs/say.as.log-{Date}.txt"); 
        }
        public static void Log(string message)
        {
            Logger.LogInformation(message);
        }
        public static void LogError(string error)
        {
            Logger.LogError(error);
        }
        public static void LogError(Exception error)
        {
            Logger.LogError(error.ToString());
        }

        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_Factory == null)
                {
                    _Factory = new LoggerFactory();
                    ConfigureLogger(_Factory);
                }
                return _Factory;
            }
            set { _Factory = value; }
        }
        public static ILogger CreateLogger() => LoggerFactory.CreateLogger("say.as.log-{Date}");
    }
}
