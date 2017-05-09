using ImageProcessor.CP5200;
using Logger;
using nightowlsign.data;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Logger.Logger;

namespace ImageProcessor.Services
{
    public class SendCommunicator : ISendCommunicator, IDisposable
    {
        public int UpLoadSuccess { get; set; }
        private readonly int TimeOut = 3600;
        private StoreAndSign _storeAndSign;
        private string _programFileDirectory;
        private readonly IMLogger _logger;

        public SendCommunicator(IMLogger logger)
        {
            _logger = logger;
        }

        public void Init(StoreAndSign storeAndSign, string programFileDirectory)
        {
            _storeAndSign = storeAndSign;
            _programFileDirectory = programFileDirectory;
        }

        public bool FilesUploadedOk()
        {
            return ConnectToSignAndUpload(_storeAndSign) == 0;
        }

        public int ConnectToSignAndUpload(StoreAndSign storeAndSign)
        {
            if (SignIsOnLine(storeAndSign.IpAddress, storeAndSign.SubMask, storeAndSign.Port))
            {
                _logger.WriteLog($"SendCommunicator - sendFiletoSign - {storeAndSign.Name}","Result");
                SendTheFiletoSign(storeAndSign.ProgramFile);
                return UpLoadSuccess;
            }
            _logger.WriteLog($"Fail send file to sign {storeAndSign.Name}","Result");
            return 99;
        }
        public void SendTheFiletoSign(string programFile)
        {
            try
            {
                foreach (
                    var programFileName in
                    Directory.GetFiles(_programFileDirectory, "*.lpb"))
                {
                    UpLoadSuccess = UploadFile(programFileName, programFile);
                    _logger.WriteLog($"{DateTime.Now}-SendComm - SendFileToSign - {programFileName} - Success={UpLoadSuccess}","Result");

                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"SendComm - SendFileToSign - Error in Sendfile to sign: {ex.Message}","Error");
            }
        }

        private int UploadFile(string programFileName, string programFile)
        {
            return Cp5200External.CP5200_Net_UploadFile(Convert.ToByte(1),
                  GetPointerFromFileName(programFileName),
                  GetPointerFromFileName($"{programFile}.lpb"));
        }

        public int RestartSign()
        {
            var success = Cp5200External.CP5200_Net_RestartApp(Convert.ToByte(1));
            _logger.WriteLog($"Restart of sign - {success}","Result");
            return success;
        }

        public bool SignIsOnLine(string ipAddress, string idCode, string port)
        {
            var signIsOnLine = false;
            try
            {
                var dwIpAddr = GetIp(ipAddress);
                var dwIdCode = GetIp(idCode);
                var nIpPort = Convert.ToInt32(port);
                if (dwIpAddr != 0 && dwIdCode != 0)
                {
                    var responseNumber = Cp5200External.CP5200_Net_Init(dwIpAddr, nIpPort, dwIdCode, TimeOut);
                    if (responseNumber == 0)
                    {
                        signIsOnLine = true;
                        var result = Cp5200External.CP5200_Net_Connect();
                        var result2 = Cp5200External.CP5200_Net_IsConnected();
                        _logger.WriteLog($"Communication established with {ipAddress}","Result");
                    }
                    else
                    {
                        _logger.WriteLog($"Communication failed with Sign {ipAddress} ","Result");
                    }
                }
                return signIsOnLine;
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"IP: {ipAddress} ,idCode: {idCode}, Port: {port} - {ex.Message}","Error");
                return false;
            }
        }

        public int Disconnect()
        {
            return Cp5200External.CP5200_Net_Disconnect();
        }

        private static uint GetIp(string strIp)
        {
            System.Net.IPAddress ipaddress = System.Net.IPAddress.Parse(strIp);
            var lIp = (uint)ipaddress?.Address;
            lIp = ((lIp & 0xFF000000) >> 24) + ((lIp & 0x00FF0000) >> 8) + ((lIp & 0x0000FF00) << 8) + ((lIp & 0x000000FF) << 24);
            return (lIp);
        }
        private static IntPtr GetPointerFromFileName(string fileName)
        {
            return Marshal.StringToHGlobalAnsi(fileName);
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
        // ~SendCommunicator() {
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
