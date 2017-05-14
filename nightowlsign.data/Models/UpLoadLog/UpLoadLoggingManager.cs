using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nightowlsign.data.Models.UpLoadLog
{


    public class UpLoadLoggingManager : IUpLoadLoggingManager
    {
        private readonly Inightowlsign_Entities _context;
        public UpLoadLoggingManager(Inightowlsign_Entities context)
        {
            _context = context;
        }

        public List<data.UpLoadLog> GetLatest()
        {
            return _context.UpLoadLogs.OrderByDescending(i => i.DateTime).Take(50).ToList();
        }

        public bool UpLoadNeeded(int storeId, string filename, DateTime lastUpdated)
        {
            var upLoadLog = _context.UpLoadLogs.Where(i => i.StoreId == storeId && i.ProgramFile == filename)
                .OrderByDescending(i => i.DateTime).FirstOrDefault();
            if (upLoadLog == null) return true;
            return upLoadLog.ResultCode != 0 || !(upLoadLog.DateTime > lastUpdated);
        }

        public void Delete(int storeId, int scheduleId)
        {
            var results = _context.UpLoadLogs.Where(u => u.StoreId == storeId && u.ScheduleId == scheduleId);
            _context.UpLoadLogs.RemoveRange(results);
            _context.SaveChanges();

        }
        public int? GetOverallStatus(int storeId, DateTime? lastUpdated, int scheduleid)
        {
            return _context.UpLoadLogs.Where(i => i.StoreId == storeId && i.ScheduleId == scheduleid).Sum(i => i.ResultCode) ?? -99;
        }

        public bool Upsert(data.UpLoadLog log)
        {
            try
            {
                var existingLog = _context.UpLoadLogs.FirstOrDefault(u => u.ProgramFile==log.ProgramFile && u.StoreId==log.StoreId && u.ScheduleId==log.ScheduleId);

                if (existingLog != null)
                {
                    _context.Entry(existingLog).CurrentValues.SetValues(log);
                }
                else
                {
                    var newLog = new data.UpLoadLog()
                    {
                        StoreId = log.StoreId,
                        ResultCode = log.ResultCode,
                        ProgramFile = log.ProgramFile,
                        DateTime = log.DateTime,
                        ScheduleId = log.ScheduleId
                    };
                    _context.UpLoadLogs.Add(newLog);
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return false;
            }
        }
    }
}
