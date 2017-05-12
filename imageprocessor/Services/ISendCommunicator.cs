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
        void Init(StoreAndSign storeAndSign, string directory);
        int UpLoadSuccess { get; }

        bool FilesUploadedOk();
        int ConnectToSignAndUpload(StoreAndSign storeAndSign);
        void SendTheFiletoSign(StoreAndSign storeAndSign);
        bool SignIsOnLine(StoreAndSign storeAndSign);
        int Disconnect();
        int RestartSign();
        bool FileNeedsToBeSent(StoreAndSign storeAndSign, string fileName);
    }
}
