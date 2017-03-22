using Logger;
using nightowlsign.data;
using System;

namespace ImageProcessor.Services
{
    public interface ISendCommunicator
    {
        void Init(StoreAndSign storeAndSign, string programFileDirectory, string playbillextension, IMLogger logger);
        bool FilesUploadedOk(string programFileName);
        bool SendFiletoSign(StoreAndSign storeAndSign);
        bool SendFiletoSign();
        bool InitComm(string ipAddress, string idCode, string port);
        uint GetIP(string signIpAddress);
        IntPtr GetPointerFromFileName(string fileName);
    }
}
