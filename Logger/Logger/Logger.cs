using System;
using nightowlsign.data;
using nightowlsign.data.Models.Logging;

namespace Logger.Logger
{
    public class MLogger : IDisposable, IMLogger
    {
        private string _fileName;
        private readonly Inightowlsign_Entities _context;
        private readonly ILoggingManager _loggingManager;

        public MLogger(Inightowlsign_Entities context, ILoggingManager loggingManager)
        {
            _context = context;
            _loggingManager = loggingManager;
        }

        public void Init (string assemblyName)
        {
            _fileName = $"{DateTime.Today.ToString("yy-MM-dd")}.txt";
            WriteLog($"Version: {assemblyName}","Startup");
        }

        public void WriteLog(string lines, DateTime datetime)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter($"c:\\logging\\{_fileName}", true))
            {
                file.WriteLine($"{lines} - {DateTime.Now}");
                file.Close();
            }
        }

        public void WriteLog(string lines)
        {
            WriteLog(lines, "logging");
         }
        public void WriteLog(string lines, string subject)
        {
            var newLog = new Logging()
            {
                Description = lines,
                DateStamp = DateTime.Now.ToLocalTime(),
                Subject = subject
            };
            _loggingManager.Insert(newLog);
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
        // ~Logger() {
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
