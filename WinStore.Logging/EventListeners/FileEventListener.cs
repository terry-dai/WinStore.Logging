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
        private List<string> _logLines;
        private ThreadPoolTimer _periodicTimer;
        private const int DefaultIntervalInSeconds = 5;
        private const string DefaultTimeFormat = "{0:yyyy-MM-dd HH\\:mm\\:ss\\:ffff}";
        private IStorageFile _logFile;
        private readonly string _logName;
        private readonly string _timeFormat;
        private readonly SemaphoreSlim _semaphore;
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
            this._logLines = new List<string>();
            this._logName = string.Format("{0}_log.csv", name.Replace(" ", "_"));
            this._timeFormat = timeFormat;
            this._semaphore = new SemaphoreSlim(1);
            this._writeIntervalInSeconds = writeInterval;
        } 
        #endregion

        #region Methods
        // Should be called right after the constructor (since constructors can't have async calls)
        public async Task InitializeAsync()
        {
            this._logFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(this._logName,
                                                                                     CreationCollisionOption.OpenIfExists);

            // We don't want to write to disk every single time a log event occurs, so let's schedule a
            // thread pool task
            _periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
            {
                await this.FlushAsync().ConfigureAwait(false);
            }, TimeSpan.FromSeconds(this._writeIntervalInSeconds));
        }

        public string GetLogName()
        {
            return this._logName;
        }

        /// <summary>
        /// Clears cache for the log and causes any cached log to be written to the file.
        /// </summary>
        /// <returns>A task that will complete when the cached log has been written intothe file.</returns>
        public async Task FlushAsync()
        {
            try
            {
                await this._semaphore.WaitAsync().ConfigureAwait(false);
                if (_logLines.Count > 0)
                {
                    await FileIO.AppendLinesAsync(_logFile, _logLines);
                    _logLines = new List<string>();
                }
            }
            finally
            {
                this._semaphore.Release();
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

            string timeStamp = string.Format(this._timeFormat, DateTime.Now);
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
                await this._semaphore.WaitAsync().ConfigureAwait(false);
                _logLines.Add(eventInfo);
            }
            catch (Exception)
            {
                // eat the exception here.
            }
            finally
            {
                this._semaphore.Release();
            }
        } 

        #endregion
    }
}
