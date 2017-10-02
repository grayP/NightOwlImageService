using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nightowlsign.data.Models.Logging
{
    public class LoggingManager : ILoggingManager
    {
        private  Inightowlsign_Entities _context;
        public LoggingManager(Inightowlsign_Entities context)
        {
            _context = context;
        }

        public List<data.Logging> GetLatest()
        {
            return _context.Logging.OrderByDescending(i => i.DateStamp).Take(50).ToList();
        }

        public bool Insert(string description, string subject)
        {
            data.Logging log = new data.Logging
            {
                Description = description,
                Subject = subject,
                DateStamp = DateTime.Now
            };
            return Insert(log);
        }
        public bool Insert(data.Logging log)
        {
            try
            {
                var newLog = new data.Logging()
                {
                    Description = log.Description.Trim(),
                    Subject = log.Subject.Trim(),
                    DateStamp = DateTime.Now.ToLocalTime()
                };
                _context.Logging.Add(newLog);
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
