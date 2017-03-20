using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public interface IMLogger
    {
        void init(string assemblyName);
        void WriteLog(string lines);
    }

    public class MLogger : IDisposable, IMLogger
    {
        private readonly string _fileName;
      

        public MLogger()
        {
            _fileName = $"{DateTime.Today.ToString("yy-MM-dd")}.txt";
        }

        public void init(string assemblyName)
        {
            WriteLog($"Version: {assemblyName}");

        }
        public void WriteLog(string lines)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter($"c:\\logging\\{_fileName}", true))
            {
                file.WriteLine($"{DateTime.Now.TimeOfDay} - {lines}");
                file.Close();
            }
            Console.WriteLine(lines);
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
