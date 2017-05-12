using System;
using System.Collections.Generic;
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
            var upLoadLog= _context.UpLoadLogs.Where(i => i.StoreId == storeId && i.ProgramFile == filename)
                .OrderByDescending(i => i.DateTime).FirstOrDefault();
            if (upLoadLog == null) return true;
            return upLoadLog.ResultCode != 0 || !(upLoadLog.DateTime > lastUpdated);
        }
        public int? GetOverallStatus(int storeId, DateTime? lastUpdated)
        {
           var result= _context.UpLoadLogs.Where(i => i.StoreId == storeId && i.DateTime > lastUpdated).Sum(i => i.ResultCode);
            if (result == null) return -99;
            return result;
        }

        public bool Insert(data.UpLoadLog log)
        {
            try
            {
                var newLog = new data.UpLoadLog()
                {
                    StoreId = log.StoreId,
                    ResultCode = log.ResultCode,
                    ProgramFile = log.ProgramFile,
                    DateTime = log.DateTime
                };
                _context.UpLoadLogs.Add(newLog);
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
