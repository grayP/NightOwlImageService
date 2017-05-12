using System;
using System.Collections.Generic;

namespace nightowlsign.data.Models.UpLoadLog
{
    public interface IUpLoadLoggingManager
    {
        List<data.UpLoadLog> GetLatest();
        bool Insert(data.UpLoadLog log);
        bool UpLoadNeeded(int storeId, string filename, DateTime lastUpdated);
        int? GetOverallStatus(int id, DateTime? lastUpdated);
    }
}