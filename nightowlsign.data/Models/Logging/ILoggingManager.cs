using System.Collections.Generic;

namespace nightowlsign.data.Models.Logging
{
    public interface ILoggingManager
    {
        List<data.Logging> GetLatest();
        bool Insert(data.Logging log);
    }
}