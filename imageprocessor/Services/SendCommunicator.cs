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
        private readonly MLogger _logger;
        private readonly string _playBillExtension;

        public SendCommunicator()
        {
           //  this._playbillFile = FindPlaybillFile();
        }

        public SendCommunicator(StoreAndSign storeAndSign, string programFileDirectory, string playbillextension, MLogger logger)
        {
            _storeAndSign = storeAndSign;
            _programFileDirectory = programFileDirectory;
            _playBillExtension = playbillextension;
            _logger = logger;
        }
        internal bool FilesUploadedOk()
        {
            return SendFiletoSign(_storeAndSign);
        }
        private string FindPlaybillFile()
        {
            DirectoryInfo di = new DirectoryInfo(_programFileDirectory);
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
            var success = false;
            try
            {
                int uploadCount = 0;
                foreach (
                    string programFileName in
                    Directory.GetFiles(_programFileDirectory, "*.lpb"))
                {
                    var uploadSuccess = Cp5200External.CP5200_Net_UploadFile(Convert.ToByte(1),
                        GetPointerFromFileName(programFileName),
                        GetPointerFromFileName(programFileName));

                    _logger.WriteLog($"UploadSuccess for {programFileName}={uploadSuccess}");
                    if (0 == uploadSuccess)
                        uploadCount++;
                }

                //if (0 ==
                //    Cp5200External.CP5200_Net_UploadFile(Convert.ToByte(1), GetPointerFromFileName(FindPlaybillFile()),
                //        GetPointerFromFileName(FindPlaybillFile())))
                //    uploadCount++;

                int restartSuccess = -1;
                if (uploadCount >= 1)
                {
                    _logger.WriteLog($"{DateTime.Now}-SendComm - SendFileToSign - Successfully uploaded {uploadCount} files and Restarted:{restartSuccess} ");
                    restartSuccess = Cp5200External.CP5200_Net_RestartApp(Convert.ToByte(1));
                    success = restartSuccess >= 0;
                }
                _logger.WriteLog($"Restart of sign - {success}");
                return success;
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"SendComm - SendFileToSign - Error in Sendfile to sign: {ex.Message}");
                return false;
            }
        }

        public bool InitComm(string ipAddress, string idCode, string port)
        {
            var signOnLine = false;
            try
            {
                uint dwIPAddr = GetIP(ipAddress);
                uint dwIDCode = GetIP(idCode);
                int nIPPort = Convert.ToInt32(port);
                if (dwIPAddr != 0 && dwIDCode != 0)
                {
                    var responseNumber = Cp5200External.CP5200_Net_Init(dwIPAddr, nIPPort, dwIDCode, TimeOut);
                    if (responseNumber == 0)
                    {
                        signOnLine = true;
                        var result = Cp5200External.CP5200_Net_Connect();
                        var result2 = Cp5200External.CP5200_Net_IsConnected();
                        _logger.WriteLog($"Communication established with {ipAddress}");
                    }
                    else
                    {
                        _logger.WriteLog($"Communication failed with Sign {ipAddress} ");
                    }
                }
                return signOnLine;
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"IP: {ipAddress} ,idCode: {idCode}, Port: {port} - {ex.Message}");
                return false;
            }
        }
        private uint GetIP(string strIp)
        {
            System.Net.IPAddress ipaddress = System.Net.IPAddress.Parse(strIp);
            uint lIp = (uint)ipaddress?.Address;
            lIp = ((lIp & 0xFF000000) >> 24) + ((lIp & 0x00FF0000) >> 8) + ((lIp & 0x0000FF00) << 8) + ((lIp & 0x000000FF) << 24);
            return (lIp);
        }
        IntPtr GetPointerFromFileName(string fileName)
        {
             // return Marshal.StringToHGlobalAnsi(fileName);
            return Marshal.StringToHGlobalAnsi("00010000.lpb");
        }

    }
}
