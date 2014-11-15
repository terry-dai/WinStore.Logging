using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace WinStore.Logging
{
    public class LoggingHelper
    {
        private static readonly FileEventListener FileListener = new FileEventListener();
        private static readonly LogEventSource LogEventSource = new LogEventSource();

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="logLevel">log level</param>
        /// <returns>Task</returns>
        public static async Task InitializeLoggingServiceAsync(EventLevel logLevel)
        {
            await FileListener.InitializeAsync().ConfigureAwait(false);
            FileListener.EnableEvents(LogEventSource, logLevel);
        }

        public static void SetEventLevel(EventLevel logLevel)
        {
            FileListener.DisableEvents(LogEventSource);
            FileListener.EnableEvents(LogEventSource, logLevel);
        }

        /// <summary>
        /// Get log filename.
        /// </summary>
        /// <returns>Filename of the log file.</returns>
        public static string GetLogFilename()
        {
            return FileListener.GetLogName();
        }


        /// <summary>
        /// Logs the message as verbose.
        /// </summary>
        /// <param name="format">A formatted message.</param>
        /// <param name="args">Parameters to be injected into the formatted message.</param>
        public static void Verbose(string format, params object[] args)
        {
            LogEventSource.Verbose(string.Format(format, args));
        }

        /// <summary>
        /// Logs the message as info.
        /// </summary>
        /// <param name="format">A formatted message.</param>
        /// <param name="args">Parameters to be injected into the formatted message.</param>
        public static void Info(string format, params object[] args)
        {
            LogEventSource.Info(string.Format(format, args));
        }

        /// <summary>
        /// Logs the message as a warning.
        /// </summary>
        /// <param name="format">A formatted message.</param>
        /// <param name="args">Parameters to be injected into the formatted message.</param>
        public static void Warn(string format, params object[] args)
        {
            LogEventSource.Warn(string.Format(format, args));
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public static void Error(Exception e)
        {
            // TODO: may need better formatted and detailed messages.
            LogEventSource.Error(e.Message);
        }

        /// <summary>
        /// Logs the error message.
        /// </summary>
        /// <param name="message">The error message</param>
        public static void Error(string message)
        {
            LogEventSource.Error(message);
        }

        /// <summary>
        /// Logs the message as an error.
        /// </summary>
        /// <param name="format">A formatted message.</param>
        /// <param name="args">Parameters to be injected into the formatted message.</param>
        public static void Error(string format, params object[] args)
        {
            LogEventSource.Error(string.Format(format, args));
        }

        /// <summary>
        /// Clears log cache and causes any cached log to be written to the file.
        /// </summary>
        /// <returns>A task that will complete when the cached log has been written intothe file.</returns>
        public static async Task FlushAsync()
        {
            await FileListener.FlushAsync().ConfigureAwait(false);
        }
    }
}
