using System;
using System.Collections.Generic;
using Logger;
using nightowlsign.data;
using nightowlsign.data.Models.SendToSign;
using nightowlsign.data.Models.StoreSign;

namespace ImageProcessor.Services
{
    public interface ICreateFilesToSend
    {
        bool UploadFileToSign();
        void GetTheImagesForSchedule();
        void DeleteOldFiles();
        void WriteImagesToDisk(List<ImageSelect> images);
        void GenerateFiles();
        void Init(StoreAndSign storeAndSign, IMLogger logger, ISendCommunicator sendCommunicator, ISendToSignManager sendToSignManager);
    }
}