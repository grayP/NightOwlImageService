using ImageProcessor.CP5200;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.CompilerServices;
using nightowlsign.data;
using Logger;

namespace ImageProcessor.Services
{
    public class SendCommunicator
    {
        private int TimeOut = 3600;
        private readonly StoreAndSign _storeAndSign;
        private readonly string _programFileDirectory;
        private mLogger _logger;

        public SendCommunicator()
        {
           //  this._playbillFile = FindPlaybillFile();
        }

        public SendCommunicator(StoreAndSign storeAndSign, string programFileDirectory, mLogger logger)
        {
            _storeAndSign = storeAndSign;
            _programFileDirectory = programFileDirectory;
            _logger = logger;
        }


        internal bool Run()
        {
            return SendFiletoSign(_storeAndSign);
        }

        private string FindPlaybillFile()
        {
            DirectoryInfo di = new DirectoryInfo(_programFileDirectory);
            var lppFile = di.EnumerateFiles().Select(f => f.Name)
                      .FirstOrDefault(f=>f.Contains(".lpp"));
            return string.Concat(_programFileDirectory,lppFile);
        }

        public bool SendFiletoSign(StoreAndSign storeAndSign)
        {
           _logger.WriteLog($"SendCommunicator - sendFiletoSign - {InitComm(storeAndSign.IpAddress, storeAndSign.SubMask, storeAndSign.Port)}{Environment.NewLine}");
            return SendFiletoSign();

        }
        public bool SendFiletoSign()
        {
            var success = false;
            try
            {
                int uploadCount = 0;
                foreach (
                    string programFileName in
                    Directory.GetFiles(_programFileDirectory, "*.lpb"))
                {
                    if (0 ==
                      Cp5200External.CP5200_Net_UploadFile(Convert.ToByte(1), GetPointerFromFileName(programFileName),
                          GetPointerFromFileName(programFileName)))
                        uploadCount++;
                }

                if (0 ==
                    Cp5200External.CP5200_Net_UploadFile(Convert.ToByte(1), GetPointerFromFileName(FindPlaybillFile()),
                        GetPointerFromFileName(FindPlaybillFile())))
                    uploadCount++;

                int restartSuccess = -1;
                if (uploadCount > 0)
                {
                    restartSuccess = Cp5200External.CP5200_Net_RestartApp(Convert.ToByte(1));
                    success = restartSuccess >= 0;
                }
                _logger.WriteLog($"SendComm - SendFileToSign - Successfully uploaded {uploadCount} files and Restarted:{restartSuccess} {Environment.NewLine}");
                return success;
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"SendComm - SendFileToSign - Error in Sendfile to sign: {ex.Message}");
                return false;
            }
        }

        public string InitComm(string ipAddress, string idCode, string port)
        {
            try
            {
                uint dwIPAddr = GetIP(ipAddress);
                uint dwIDCode = GetIP(idCode);
                int nIPPort = Convert.ToInt32(port);
                if (dwIPAddr != 0 && dwIDCode != 0)
                {
                    var responseNumber = Cp5200External.CP5200_Net_Init(dwIPAddr, nIPPort, dwIDCode, TimeOut);
                    if (responseNumber == 0)
                        return $"Communication established with {ipAddress}";
                }
                return $"Communication failed with Sign {ipAddress} ";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private uint GetIP(string strIp)
        {
            System.Net.IPAddress ipaddress = System.Net.IPAddress.Parse(strIp);
            uint lIp = (uint)ipaddress.Address;
            lIp = ((lIp & 0xFF000000) >> 24) + ((lIp & 0x00FF0000) >> 8) + ((lIp & 0x0000FF00) << 8) + ((lIp & 0x000000FF) << 24);
            return (lIp);
        }
        IntPtr GetPointerFromFileName(string fileName)
        {
            return Marshal.StringToHGlobalAnsi(fileName);
        }

    }
}
