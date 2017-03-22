using ImageProcessor.CP5200;
using Logger;
using nightowlsign.data;
using System;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ImageProcessor.Services
{
    public class SendCommunicator : ISendCommunicator
    {
        private int TimeOut = 3600;
        private StoreAndSign _storeAndSign;
        private string _programFileDirectory;
        private IMLogger _logger;
        private string _playBillExtension;
        private string _programFileName;

        public int UpLoadSuccess { get; set; }
        public int RestartSuccess { get; set; }

        public void Init(StoreAndSign storeAndSign, string programFileDirectory, string playbillextension, IMLogger logger)
        {
            _storeAndSign = storeAndSign;
            _programFileDirectory = programFileDirectory;
            _playBillExtension = playbillextension;
            _logger = logger;
        }

        public bool FilesUploadedOk(string programFileName)
        {
            _programFileName = programFileName;
            return SendFiletoSign(_storeAndSign);
        }
        private string FindPlaybillFile()
        {
            var di = new DirectoryInfo(_programFileDirectory);
            var lppFile = di.EnumerateFiles().Select(f => f.Name)
                      .FirstOrDefault(f => f.Contains(_playBillExtension));
            return string.Concat(_programFileDirectory, lppFile);
        }

        public bool SendFiletoSign(StoreAndSign storeAndSign)
        {
            if (InitComm(storeAndSign.IpAddress, storeAndSign.SubMask, storeAndSign.Port))
            {
                _logger.WriteLog($"SendCommunicator - sendFiletoSign - {storeAndSign.Name}");
                return SendFiletoSign();
            }
            _logger.WriteLog($"Fail send file to sign {storeAndSign.Name}");
            return false;

        }
        public bool SendFiletoSign()
        {
             try
            {
                    UpLoadSuccess = Cp5200External.CP5200_Net_UploadFile(Convert.ToByte(1),
                        GetPointerFromFileName(_programFileName),
                        GetPointerFromFileName(_programFileName));

                    _logger.WriteLog($"UploadSuccess for {_programFileName}={UpLoadSuccess}");
                    if (UpLoadSuccess>=0)
                    {
                        _logger.WriteLog($"{DateTime.Now}-SendComm - SendFileToSign - Successfully uploaded {_programFileName} file and Restarted:{RestartSuccess} ");
                        RestartSuccess = Cp5200External.CP5200_Net_RestartApp(Convert.ToByte(1));
                    }
                return UpLoadSuccess>=0;
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"SendComm - SendFileToSign - Error in Sendfile to sign: {ex.Message}");
                return false;
            }
        }
 
        public bool InitComm(string ipAddress, string idCode, string port)
        {
             try
            {
                var dwIpAddr = GetIP(ipAddress);
                var dwIdCode = GetIP(idCode);
                var nIpPort = Convert.ToInt32(port);
                if (dwIpAddr == 0 || dwIdCode == 0) return false;
                var responseNumber = Cp5200External.CP5200_Net_Init(dwIpAddr, nIpPort, dwIdCode, TimeOut);
                _logger.WriteLog(responseNumber==0 ? $"Communication established with {ipAddress}" : $"Communication failed with Sign {ipAddress} - Response: {responseNumber}");

                return responseNumber == 0;
              
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"IP: {ipAddress} ,idCode: {idCode}, Port: {port} - {ex.Message}");
                return false;
            }
        }

        public uint GetIP(string signIpAddress)
        {
            var ipaddress = System.Net.IPAddress.Parse(signIpAddress);
            var lIp = (uint)ipaddress?.Address;
            lIp = ((lIp & 0xFF000000) >> 24) + ((lIp & 0x00FF0000) >> 8) + ((lIp & 0x0000FF00) << 8) + ((lIp & 0x000000FF) << 24);
            return (lIp);
        }

        public IntPtr GetPointerFromFileName(string fileName)
        {
            return Marshal.StringToHGlobalAnsi(_programFileName);
        }

    }
}
