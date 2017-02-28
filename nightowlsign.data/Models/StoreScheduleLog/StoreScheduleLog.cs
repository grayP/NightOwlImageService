using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;


namespace nightowlsign.data.Models.StoreScheduleLog
{
    public class StoreScheduleLogManager
    {
        public string errorMessage { get; set; }
        public data.StoreScheduleLog entity { get; set; }
        public StoreScheduleLogManager()
        {
        }


        public List<data.StoreScheduleLog> Get(data.StoreScheduleLog storeScheduleLog)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                var ret = db.StoreScheduleLogs.OrderBy(x => x.ScheduleName).ToList<data.StoreScheduleLog>();
                ret = ret.FindAll(p => p.ScheduleName.ToLower().StartsWith(storeScheduleLog.ScheduleName));
                return ret;
            }
        }


        private data.StoreScheduleLog GetStoreScheduleLog(int scheduleStoreLogId)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                return db.StoreScheduleLogs.Find(scheduleStoreLogId);
            }
        }


        public bool Insert()
        {
            try
            {
                using (nightowlsign_Entities db = new nightowlsign_Entities())
                {
                    data.StoreScheduleLog newStoreScheduleLog = new data.StoreScheduleLog()
                    {
                        ScheduleName = entity.ScheduleName.Trim(),
                        DateInstalled = DateTime.Now,
                        StoreId = entity.StoreId,
                        InstalledOk = true,
                        ScheduleId = entity.ScheduleId
                    };
                    db.StoreScheduleLogs.Add(newStoreScheduleLog);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }


        public bool Delete(data.StoreScheduleLog entity)
        {
            bool ret = false;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                db.StoreScheduleLogs.Attach(entity);
                db.StoreScheduleLogs.Remove(entity);
                db.SaveChanges();
                ret = true;
            }
            return ret;
        }
    }

}
