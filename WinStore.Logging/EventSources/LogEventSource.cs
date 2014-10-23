using System.Diagnostics.Tracing;

namespace WinStore.Logging
{
    public sealed class LogEventSource : EventSource
    {
        private static readonly LogEventSource LogSource = new LogEventSource();

        public const int VerboseLevel = (int)EventLevel.Verbose, InformationalLevel = (int)EventLevel.Informational, WarningLevel = (int)EventLevel.Warning, ErrorLevel = (int)EventLevel.Error, CriticalLevel = (int)EventLevel.Critical;

        [Event(VerboseLevel, Level = EventLevel.Verbose)]
        public void Verbose(string message)
        {
            this.WriteEvent(VerboseLevel, message);
        }

        [Event(InformationalLevel, Level = EventLevel.Informational)]
        public void Info(string message)
        {
            this.WriteEvent(InformationalLevel, message);
        }

        [Event(WarningLevel, Level = EventLevel.Warning)]
        public void Warn(string message)
        {
            this.WriteEvent(WarningLevel, message);
        }

        [Event(ErrorLevel, Level = EventLevel.Error)]
        public void Error(string message)
        {
            this.WriteEvent(ErrorLevel, message);
        }

        [Event(CriticalLevel, Level = EventLevel.Critical)]
        public void Critical(string message)
        {
            this.WriteEvent(CriticalLevel, message);
        }
        
        public void LogMessage(EventLevel eventLevel, string message)
        {
            this.WriteEvent((int)eventLevel, message);
        }
    }
}
