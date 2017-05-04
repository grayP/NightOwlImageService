using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;


namespace nightowlsign.data.Models.StoreScheduleLog
{
    public class StoreScheduleLogManager
    {
        public string ErrorMessage { get; set; }
        public data.StoreScheduleLog Entity { get; set; }
        public StoreScheduleLogManager()
        {
        }

        public StoreScheduleLogManager(StoreAndSign storeAndSign)
        {
            Entity = new data.StoreScheduleLog()
            {
                DateInstalled = DateTime.Now,
                ScheduleName = storeAndSign.CurrentSchedule.Name,
                ScheduleId = storeAndSign.CurrentSchedule.Id,
                InstalledOk = true,
                StoreId = storeAndSign.id
            };
        }

        public List<data.StoreScheduleLog> Get(data.StoreScheduleLog storeScheduleLog)
        {
            using (var db = new nightowlsign_Entities())
            {
                var ret = db.StoreScheduleLogs.OrderBy(x => x.ScheduleName).ToList<data.StoreScheduleLog>();
                ret = ret.FindAll(p => p.ScheduleName.ToLower().StartsWith(storeScheduleLog.ScheduleName));
                return ret;
            }
        }


        private data.StoreScheduleLog GetStoreScheduleLog(int scheduleStoreLogId)
        {
            using (var db = new nightowlsign_Entities())
            {
                return db.StoreScheduleLogs.Find(scheduleStoreLogId);
            }
        }

        public bool Insert()
        {
            try
            {
                using (var db = new nightowlsign_Entities())
                {
                    data.StoreScheduleLog storeScheduleLog = new data.StoreScheduleLog()
                    {
                        ScheduleName = Entity.ScheduleName.Trim(),
                        DateInstalled = DateTime.Now,
                        StoreId = Entity.StoreId,
                        InstalledOk = true,
                        ScheduleId = Entity.ScheduleId
                    };
                    db.StoreScheduleLogs.Add(storeScheduleLog);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }


        public bool Delete(data.StoreScheduleLog entity)
        {
            using (var db = new nightowlsign_Entities())
            {
                db.StoreScheduleLogs.Attach(entity);
                db.StoreScheduleLogs.Remove(entity);
                db.SaveChanges();
                return true;
            }
        }
    }

}
