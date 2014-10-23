using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System.Threading;

namespace WinStore.Logging
{
    /// <summary>
    /// The periodically writting file event listener.
    /// Replaced the lock() with a SemaphoreSlim.
    /// http://cryclops.com/2014/01/not-so-stupid-simple-logging-for-windows-store-apps-in-cvb/
    /// </summary>
    public class FileEventListener : EventListener
    {
        #region Fields
        private List<string> logLines;
        private ThreadPoolTimer periodicTimer;
        private const int DefaultIntervalInSeconds = 5;
        private const string DefaultTimeFormat = "{0:yyyy-MM-dd HH\\:mm\\:ss\\:ffff}";
        private IStorageFile logFile;
        private readonly string logName;
        private readonly string timeFormat;
        private readonly SemaphoreSlim semaphore;
        private readonly int _writeIntervalInSeconds; 
        #endregion

        #region Constructors
        public FileEventListener()
            : this(Package.Current.Id.Name)
        {
        }

        public FileEventListener(string name)
            : this(name, DefaultTimeFormat, DefaultIntervalInSeconds)
        {

        }

        public FileEventListener(string name, string timeFormat, int writeInterval)
        {
            this.logLines = new List<string>();
            this.logName = string.Format("{0}_log.csv", name.Replace(" ", "_"));
            this.timeFormat = timeFormat;
            this.semaphore = new SemaphoreSlim(1);
            this._writeIntervalInSeconds = writeInterval;
        } 
        #endregion

        #region Methods
        // Should be called right after the constructor (since constructors can't have async calls)
        public async Task InitializeAsync()
        {
            this.logFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(this.logName,
                                                                                     CreationCollisionOption.OpenIfExists);

            // We don't want to write to disk every single time a log event occurs, so let's schedule a
            // thread pool task
            periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
            {
                await this.FlushAsync().ConfigureAwait(false);
            }, TimeSpan.FromSeconds(this._writeIntervalInSeconds));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetLogName()
        {
            return this.logName;
        }

        /// <summary>
        /// Clears cache for the log and causes any cached log to be written to the file.
        /// </summary>
        /// <returns>A task that will complete when the cached log has been written intothe file.</returns>
        public async Task FlushAsync()
        {
            try
            {
                await this.semaphore.WaitAsync().ConfigureAwait(false);
                if (logLines.Count > 0)
                {
                    await FileIO.AppendLinesAsync(logFile, logLines);
                    logLines = new List<string>();
                }
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            string payload = "";

            if (eventData != null && eventData.Message != null)
            {
                payload = string.Format(eventData.Message, eventData.Payload.ToArray());
            }
            else if (eventData.Payload.Count > 0)
            {
                payload = eventData.Payload[0].ToString();
            }

            string timeStamp = string.Format(this.timeFormat, DateTime.Now);
            string threadInfo = string.Format("Thread:{0}", Environment.CurrentManagedThreadId);
            var eventInfo = String.Format("{0},{1},{2},{3}",
                                          timeStamp,
                                          eventData.Level,
                                          threadInfo,
                                          payload);
            this.CacheLogAsync(eventInfo);
        }

        private async void CacheLogAsync(string eventInfo)
        {
            try
            {
                await this.semaphore.WaitAsync().ConfigureAwait(false);
                logLines.Add(eventInfo);
            }
            catch (Exception)
            {
                // eat the exception here.
            }
            finally
            {
                this.semaphore.Release();
            }
        } 

#if TEST
#endif
        #endregion
    }
}
