using System;
using System.Collections.Generic;
using nightowlsign.data.Models;

namespace ImageProcessor.Services
{
    public interface ICreateFilesToSend
    {
        bool Run();

        void GetTheImagesForSchedule();
        void DeleteOldFiles();

        void WriteImagesToDisk(List<ImageSelect> images);
        void GenerateFiles(string currentScheduleName);
    }
}