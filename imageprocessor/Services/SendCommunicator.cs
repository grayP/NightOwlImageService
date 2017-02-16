using ImageProcessor.CP5200;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using nightowlsign.data.Models.StoreSignDto;
using System.IO;
using System.Web;

namespace ImageProcessor.Services
{
    public class SendCommunicator
    {
        private int TimeOut = 3600;
        //public string _playbillFile { get; set; }
        private const string ProgramFileDirectory = "~/playBillFiles/";

        //public SendCommunicator(string PlaybillFile)
        //{
        //    // this.ProgramFiles = ProgramFiles;
        //    this._playbillFile = PlaybillFile;
        //}
        public SendCommunicator()
        {
           //  this._playbillFile = FindPlaybillFile();
        }

        private string FindPlaybillFile()
        {
            DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath(ProgramFileDirectory));
            var lppFile = di.EnumerateFiles().Select(f => f.Name)
                      .FirstOrDefault(f=>f.Contains(".lpp"));
            return string.Concat(HttpContext.Current.Server.MapPath(ProgramFileDirectory),lppFile);

        }

        public string SendFiletoSign(List<StoreSignDTO> StoresForSchedule)
        {
            string displayMessage = string.Empty;
            foreach (var storeSign in StoresForSchedule)
            {
                displayMessage = string.Format(InitComm(storeSign.IPAddress, storeSign.SubMask, storeSign.Port), Environment.NewLine);
                displayMessage += string.Format(SendFiletoSign(), Environment.NewLine);
            }
            return displayMessage;
        }
        public string SendFiletoSign()
        {
            try
            {
                int uploadCount = 0;
                foreach (
                    string programFileName in
                    Directory.GetFiles(HttpContext.Current.Server.MapPath(ProgramFileDirectory), "*.lpb"))
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
                }
                return string.Format("Successfully uploaded {0} files and Restarted:{1} {2}", uploadCount, restartSuccess, Environment.NewLine);
            }
            catch (Exception ex)
            {
                return ex.Message;
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
                        return string.Format("Communication established with {0} ", ipAddress);
                }
                return string.Format("Communication failed with Sign ");
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
