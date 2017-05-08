using System.Collections.Generic;
using nightowlsign.data;
using nightowlsign.data.Models;

namespace ImageProcessor.Services
{
    public interface IScreenImageManager
    {
        string PlaybillFileName { get; set; }
        bool Successfull { get; set; }
        StoreAndSign StoreAndSign { get; set; }
        int FileUploadResultCode(StoreAndSign storeAndSign);
        List<ImageSelect> GetImages(int scheduleId);
        void DeleteOldFiles(string directoryName, string extension);
        void WriteImagesToDisk(IEnumerable<ImageSelect> images);
        void GeneratetheProgramFiles(string scheduleName, string programFile, Sign sign);
        void DeleteOldProgramFile(string fileAndPath);
        void GeneratethePlayBillFile(string scheduleName);
        void SaveImageToFile(string sCounter, ImageSelect image);
        string GeneratePlayBillFileName(string scheduleName);
        string GenerateProgramFileName(string programFile);
    }
}