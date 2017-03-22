using System;
using System.Collections.Generic;
using System.Linq;
using nightowlsign.data;

namespace nightowlsign.data.Models.StoreScheduleLog
{
    public class StoreScheduleLogManager : IStoreScheduleLogManager
    {
        public object ErrorMessage { get; set; }
        public data.StoreScheduleLog Entity { get; set; }
        public StoreScheduleLogManager()
        {
            Entity= new data.StoreScheduleLog();
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


        public data.StoreScheduleLog GetStoreScheduleLog(int scheduleStoreLogId)
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
                        ScheduleName = Entity.ScheduleName.Trim(),
                        DateInstalled = DateTime.Now,
                        StoreId = Entity.StoreId,
                        InstalledOk = true,
                        ScheduleId = Entity.ScheduleId
                    };
                    db.StoreScheduleLogs.Add(newStoreScheduleLog);
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

        public void UpdateInstallLog(StoreAndSign storeAndSign)
        {
            Entity.DateInstalled=DateTime.Now.ToLocalTime();
            Entity.ScheduleName = storeAndSign.CurrentSchedule.Name;
            Entity.ScheduleId = storeAndSign.CurrentSchedule.Id;
            Entity.StoreId = storeAndSign.id;
            Entity.InstalledOk = true;

        }
    }

}
