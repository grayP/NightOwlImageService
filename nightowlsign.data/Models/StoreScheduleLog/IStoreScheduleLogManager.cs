using System;
using System.Collections.Generic;

namespace nightowlsign.data.Models.StoreScheduleLog
{
    public interface IStoreScheduleLogManager
    {
        void Init(StoreAndSign storeAndSign);
        string ErrorMessage { get; set; }
        data.StoreScheduleLog Entity { get; set; }
        List<data.StoreScheduleLog> Get(data.StoreScheduleLog storeScheduleLog);
        data.StoreScheduleLog GetStoreScheduleLog(int scheduleStoreLogId);
        bool Insert(DateTime datetime);
        bool Delete(data.StoreScheduleLog entity);
    }
}