using nightowlsign.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor.Services
{
    public interface ISendCommunicator
    {
        bool FilesUploadedOk();
        int ConnectToSign(StoreAndSign storeAndSign);
        void SendTheFiletoSign(string programFile);
        bool SignIsOnLine(string ipAddress, string idCode, string port);
        int Disconnect();
    }
}
