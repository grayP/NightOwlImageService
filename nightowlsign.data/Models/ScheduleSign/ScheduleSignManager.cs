using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nightowlsign.data.Models
{
    public class ScheduleSignManager
    {
        public ScheduleSignManager()
        {
            ValidationErrors = new List<KeyValuePair<string, string>>();

        }
        //Properties
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public List<int?> Get(data.Schedule Entity)
        {
            int scheduleId = Entity.Id;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                var query = (from c in db.ScheduleSigns
                             where c.ScheduleID == scheduleId
                             select c.SignId);
                return query.ToList();
            }

        }

        public List<SelectListItem> GetAllSigns(int signId)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                var query = (from s in db.Signs orderby (s.Width/s.Height) descending 
                             select new SelectListItem() {SignId = s.id, Model= s.Model, AspectRatio = (decimal)s.Width/s.Height});
                return query.ToList();
            }
        }


        public Sign Find(int signId)
        {
            Sign ret = null;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                ret = db.Signs.Find(signId);
            }
            return ret;

        }

        public void UpdateSignList(SelectListItem signSelect, data.Schedule schedule)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
               ScheduleSign scheduleSign = new ScheduleSign()
                {
                    Id = signSelect.Id,
                    SignId = signSelect.SignId,
                    ScheduleID = schedule.Id
                };
                if (signSelect.selected)
                {
                    db.Set<ScheduleSign>().AddOrUpdate(scheduleSign);
                    db.SaveChanges();
                }
                else
                {
                        List<ScheduleSign> scheduleSigns =
                        db.ScheduleSigns.Where(
                            x => x.SignId.Value == scheduleSign.SignId.Value && x.ScheduleID == scheduleSign.ScheduleID).ToList();
                    foreach (ScheduleSign sSign in scheduleSigns)
                    {
                        db.ScheduleSigns.Attach(sSign);
                        db.ScheduleSigns.Remove(sSign);
                        db.SaveChanges();
                    }
                   
                }
            }
        }

        internal bool IsSelected(int? signId, int scheduleId)
        {
            bool ret = false;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                ret = db.ScheduleSigns.Any(x => x.ScheduleID == scheduleId && x.SignId == signId);
            }
            return ret;
        }

        internal ScheduleSign GetValues(SelectListItem signSelect)
        {
            ScheduleSign scheduleSign = null;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                scheduleSign =
                    db.ScheduleSigns.FirstOrDefault(x => x.ScheduleID == signSelect.ScheduleId && x.SignId == signSelect.SignId);
            }
            return scheduleSign;
        }
    }
}
