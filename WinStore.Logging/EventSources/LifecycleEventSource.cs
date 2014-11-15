using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace WinStore.Logging
{
    public class LifecycleEventSource : EventSource
    {
        public static readonly LifecycleEventSource Log = new LifecycleEventSource();

        public static bool IsInitialized
        {
            get
            {
                return LifecycleEventSource.Log != null;
            }
        }

        [Event(EventIds.LifecycleStartEvent, Message = "'{0}' App starting on {1} with version {2}.")]
        public void AppStarting(string appName, string platform, string version)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(EventIds.LifecycleStartEvent, appName, platform, version);
            }
        }

        [Event(EventIds.LifecycleSuspendEvent, Message = "App suspending.")]
        public void AppSuspending()
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(EventIds.LifecycleSuspendEvent);
            }
        }

        [Event(EventIds.LifecycleResumeEvent, Message = "App resuming.")]
        public void AppResuming()
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(EventIds.LifecycleResumeEvent);
            }
        }

        public static void ApplicationStarting()
        {
            if (LifecycleEventSource.IsInitialized)
            {
                string appName = Package.Current.DisplayName;
                string platform = "Windows 8.1";
                // Get app version
                var v = Package.Current.Id.Version;
                string version = string.Format("{0}.{1}.{2}.{3}", v.Major, v.Minor, v.Build, v.Revision);
                Log.AppStarting(appName, platform, version);
            }
        }

        public static void ApplicationSuspending()
        {
            if (LifecycleEventSource.IsInitialized)
            {
                Log.AppSuspending();
            }
        }

        public static void ApplicationResuming()
        {
            if (LifecycleEventSource.IsInitialized)
            {
                Log.AppResuming();
            }
        }
    }
}
