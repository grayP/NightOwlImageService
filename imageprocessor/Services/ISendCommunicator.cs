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
        int UpLoadSuccess { get; }

        bool FilesUploadedOk(StoreAndSign storeAndSign, string programFileDirectory);
        int ConnectToSignAndUpload(StoreAndSign storeAndSign, string programDir);
        void SendTheFiletoSign(StoreAndSign storeAndSign, string programDir);
        bool SignIsOnLine(StoreAndSign storeAndSign);
        int Disconnect();
        int RestartSign();
        bool FileNeedsToBeSent(StoreAndSign storeAndSign, string fileName);
    }
}
