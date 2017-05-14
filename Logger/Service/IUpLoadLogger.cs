
using System;

namespace Logger.Service
{
    public interface IUpLoadLogger
    {
        void WriteLog(int storeId, int successCode, string programFile, int scheduleId);
        bool FileNeedsToBeUploaded(int storeid, string fileName, DateTime lastUpdated, int scheduleId);
    }
}