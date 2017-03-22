using System;
using System.Collections.Generic;
using System.Linq;


namespace nightowlsign.data.Models.Schedule
{
    public class ScheduleManager : IScheduleManager
    {
        public ScheduleManager()
        {
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }
        //Properties
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public List<ScheduleAndSign> Get(data.Schedule entity)
        {
            var ret = new List<ScheduleAndSign>();
            using (var db = new nightowlsign_Entities())
            {
                ret = db.ScheduleAndSigns.OrderBy(x => x.Id).ToList<ScheduleAndSign>();
            }
            if (entity.SignId > 0)
            {
                ret = ret.FindAll(p => p.SignId.Equals(entity.SignId));
            }
            if (!string.IsNullOrEmpty(entity.Name))
            {
                ret = ret.FindAll(p => p.Name.ToLower().StartsWith(entity.Name));
            }
            return ret;
        }

        public data.Schedule Find(int scheduleId)
        {
            using (var db = new nightowlsign_Entities())
            {
                return db.Schedules.Find(scheduleId);
            }
        }

        public bool Validate(data.Schedule entity)
        {
            ValidationErrors.Clear();
            if (!string.IsNullOrEmpty(entity.Name))
            {
                if (entity.Name.ToLower() == entity.Name)
                {
                    // ValidationErrors.Add(new KeyValuePair<string, string>("Schedule Name", "Schedule Name cannot be all lower case"));
                }
            }
            return (ValidationErrors.Count == 0);
        }


        public bool Update(data.Schedule entity)
        {
            if (Validate(entity))
            {
                try
                {
                    using (var db = new nightowlsign_Entities())
                    {
                        db.Schedules.Attach(entity);
                        var modifiedSchedule = db.Entry(entity);
                        modifiedSchedule.Property(e => e.Name).IsModified = true;
                        modifiedSchedule.Property(e => e.StartDate).IsModified = true;
                        modifiedSchedule.Property(e => e.EndDate).IsModified = true;
                        modifiedSchedule.Property(e => e.Monday).IsModified = true;
                        modifiedSchedule.Property(e => e.Tuesday).IsModified = true;
                        modifiedSchedule.Property(e => e.Wednesday).IsModified = true;
                        modifiedSchedule.Property(e => e.Thursday).IsModified = true;
                        modifiedSchedule.Property(e => e.Friday).IsModified = true;
                        modifiedSchedule.Property(e => e.Saturday).IsModified = true;
                        modifiedSchedule.Property(e => e.Sunday).IsModified = true;
                        modifiedSchedule.Property(e => e.DefaultPlayList).IsModified = true;
                        modifiedSchedule.Property(e => e.StartTime).IsModified = true;
                        modifiedSchedule.Property(e => e.EndTime).IsModified = true;
                        modifiedSchedule.Property(e => e.Valid).IsModified = true;
                        modifiedSchedule.Property(e => e.SignId).IsModified = true;

                        db.SaveChanges();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException);
                    return false;
                }
            }
            return false;
        }

        public bool Insert(data.Schedule entity)
        {
            try
            {
                if (Validate(entity))
                {
                    using (var db = new nightowlsign_Entities())
                    {
                        data.Schedule newSchedule = new data.Schedule()
                        {
                            Name = entity.Name,
                            StartDate = entity.StartDate ?? DateTime.Now,
                            EndDate = entity.EndDate ?? DateTime.Now.AddMonths(3),
                            Monday = entity.Monday,
                            Tuesday = entity.Tuesday,
                            Wednesday = entity.Wednesday,
                            Thursday = entity.Thursday,
                            Friday = entity.Friday,
                            Saturday = entity.Saturday,
                            Sunday = entity.Sunday,
                            DefaultPlayList = entity.DefaultPlayList,
                            StartTime = entity.StartTime,
                            EndTime = entity.EndTime,
                            Valid = entity.Valid,
                            SignId = entity.SignId,
                            LastUpdated = DateTime.Now.ToLocalTime()
                        };

                        db.Schedules.Add(newSchedule);
                        db.SaveChanges();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return false;
            }
            return false;
        }


        public bool Delete(data.Schedule entity)
        {
            using (var db = new nightowlsign_Entities())
            {
                db.Schedules.Attach(entity);
                db.Schedules.Remove(entity);
                db.SaveChanges();
                return true;
            }
        }
    }

}
