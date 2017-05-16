using ImageProcessor.CP5200;
using Logger;
using nightowlsign.data;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using Logger.Service;
using nightowlsign.data.Models.UpLoadLog;

namespace ImageProcessor.Services
{
    public class SendCommunicator : ISendCommunicator, IDisposable
    {
        public int UpLoadSuccess { get; set; }

        private const int TimeOut = 3600;
        private StoreAndSign _storeAndSign;
        private string _programFileDirectory;
        private readonly IGeneralLogger _logger;
        private readonly IUpLoadLogger _upLoadLogger;

        public SendCommunicator(IGeneralLogger logger, IUpLoadLogger uploadLogger)
        {
            _logger = logger;
            _upLoadLogger = uploadLogger;
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
            if (SignIsOnLine(storeAndSign))
            {
                SendTheFiletoSign(storeAndSign);
                return UpLoadSuccess;
            }
            _logger.WriteLog($"Fail send file to sign {storeAndSign.Name}", "Result");
            return 99;
        }
        public void SendTheFiletoSign(StoreAndSign storeAndSign)
        {
            try
            {
                foreach (
                    var programFileName in
                    Directory.GetFiles(_programFileDirectory, "*.lpb"))
                {
                    var fileName = Path.GetFileNameWithoutExtension(programFileName);
                    if (!FileNeedsToBeSent(storeAndSign, fileName)) continue;
                    for (var i = 0; i < 5; i++)
                    {
                        UpLoadSuccess = UploadFile(programFileName, fileName);
                        _logger.WriteLog($"SendFileToSign {i}, {fileName}", "Result");
                        if (UpLoadSuccess == 0) break;
                    }
                    _upLoadLogger.WriteLog(storeAndSign.id, UpLoadSuccess, fileName, storeAndSign.CurrentSchedule.Id);
                    _logger.WriteLog(
                        $"SendFileToSign -{fileName}, {storeAndSign.NumImages} img. Result:{UpLoadSuccess}",
                        "Result");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"SendFileToSign - Error in Sendfile to sign: {ex.Message}", "Error");
            }
        }

        public bool FileNeedsToBeSent(StoreAndSign storeAndSign, string fileName)
        {
            return _upLoadLogger.FileNeedsToBeUploaded(storeAndSign.id, fileName, storeAndSign.CurrentSchedule.LastUpdated ?? DateTime.Now.ToUniversalTime(), storeAndSign.CurrentSchedule.Id);
          
        }

        private int UploadFile( string programFileName, string programFile)
        {
                return Cp5200External.CP5200_Net_UploadFile(Convert.ToByte(1),
                    GetPointerFromFileName(programFileName),
                    GetPointerFromFileName($"{programFile}.lpb"));
            
        }

        public int RestartSign()
        {
            var success = Cp5200External.CP5200_Net_RestartApp(Convert.ToByte(1));
            _logger.WriteLog($"Restart of sign - {success}", "Result");
            return success;
        }

        public bool SignIsOnLine(StoreAndSign storeAndSign)   
        {
            var signIsOnLine = false;
            try
            {
                var dwIpAddr = GetIp(storeAndSign.IpAddress);
                var dwIdCode = GetIp(storeAndSign.SubMask);
                var nIpPort = Convert.ToInt32(storeAndSign.Port);
                if (dwIpAddr != 0 && dwIdCode != 0)
                {
                    var responseNumber = Cp5200External.CP5200_Net_Init(dwIpAddr, nIpPort, dwIdCode, TimeOut);
                    if (responseNumber == 0)
                    {
                        signIsOnLine = true;
                        _logger.WriteLog($"Comms established with {storeAndSign.IpAddress}, {storeAndSign.Name}", "Result");
                        var bind = Cp5200External.CP5200_Net_SetBindParam(dwIpAddr, nIpPort);
                        var result = Cp5200External.CP5200_Net_Connect();
                        var result2 = Cp5200External.CP5200_Net_IsConnected();
                    }
                    else
                    {
                        _logger.WriteLog($"Communication failed with Sign {storeAndSign.IpAddress} ", "Result");
                    }
                }
                return signIsOnLine;
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"IP: {storeAndSign.IpAddress} ,idCode: {storeAndSign.SubMask}, Port: {storeAndSign.Port} - {ex.Message}", "Error");
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
