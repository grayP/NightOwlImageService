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
        bool Run();

        void GetTheImagesForSchedule();
        void DeleteOldFiles();

        void WriteImagesToDisk(List<ImageSelect> images);
        void GenerateFiles(string currentScheduleName);
        void Init(StoreAndSign storeAndSign, MLogger logger, ISendCommunicator sendCommunicator, ISendToSignManager sendToSignManager);
    }
}