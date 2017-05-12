using System;
using nightowlsign.data;
using nightowlsign.data.Models.UpLoadLog;

namespace Logger.Service
{
    public class UpLoadLogger : IDisposable, IUpLoadLogger
    {
        private readonly IUpLoadLoggingManager _upLoadLoggingManager;
        public UpLoadLogger(IUpLoadLoggingManager upLoadLoggingManager)
        {
            _upLoadLoggingManager = upLoadLoggingManager;
        }

        public void WriteLog(int storeId, int successCode, string programFile)
        {
            try
            {
                var newLog = new UpLoadLog()
                {
                    StoreId = storeId,
                    ResultCode = successCode,
                    ProgramFile = programFile,
                    DateTime = DateTime.Now.ToLocalTime()
                };
                _upLoadLoggingManager.Insert(newLog);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public bool FileNeedsToBeUploaded(int storeid, string fileName, DateTime lastUpdated)
        {
            return _upLoadLoggingManager.UpLoadNeeded(storeid, fileName, lastUpdated);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UpLoadLogger() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
