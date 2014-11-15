using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinStore.Logging.Test.Mock
{
    public class EventSourceMock : EventSource
    {
        public void LogMessage(EventLevel eventLevel, string message)
        {
            this.WriteEvent((int)eventLevel, message);
        }
    }
}
