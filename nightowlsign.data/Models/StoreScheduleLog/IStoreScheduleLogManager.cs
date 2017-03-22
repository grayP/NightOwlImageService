using System.Collections.Generic;

namespace nightowlsign.data.Models.StoreScheduleLog
{
    public interface IStoreScheduleLogManager
    {
        object ErrorMessage { get; set; }
        List<data.StoreScheduleLog> Get(data.StoreScheduleLog storeScheduleLog);
        data.StoreScheduleLog GetStoreScheduleLog(int scheduleStoreLogId);
        bool Insert();
        bool Delete(data.StoreScheduleLog entity);
        void UpdateInstallLog(StoreAndSign storeandSign);
    }
}