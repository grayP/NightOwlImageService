using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;


namespace nightowlsign.data.Models.Stores
{
    public class StoreManager
    {
        private data.Schedule defaultSchedule;
        private Sign defaultSign;
        public StoreManager()
        {
            ValidationErrors = new List<KeyValuePair<string, string>>();
            defaultSchedule = new data.Schedule
            {
                Name = "No default playlist",
                StartDate = null,
                EndDate = null,
                Id = 0,
                SignId = 0
            };
            defaultSign = new Sign
            {
                Model = "None",
                Width = 0,
                Height = 1,
                id = 0,
                StoreId = 0,
                InstallDate = DateTime.Now
            };
        }
        //Properties
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public List<StoreAndSign> Get(Store Entity)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                var ret = db.StoreAndSigns.OrderBy(x => x.Name).ToList<StoreAndSign>();
                if (!string.IsNullOrEmpty(Entity.Name))
                {
                    ret = ret.FindAll(p => p.Name.ToLower().StartsWith(Entity.Name));
                }
                GetSchedulesAndSign(ret);
                return ret;
            }
        }

        private void GetSchedulesAndSign(List<StoreAndSign> storeList)
        {
            foreach (var store in storeList)
            {
                store.CurrentSchedule = GetCurrentSchedule(store.id);
                store.LastInstalled = GetInstalledSchedule(store.id);
                store.Sign = GetSign(store.SignId ?? 0);
            }
        }

        private Sign GetSign(int signId)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                return db.Signs.Find(signId);
            }
        }

        private data.Schedule GetInstalledSchedule(int storeId)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                var ret = (from s in db.StoreScheduleLogs
                           where s.StoreId == storeId
                           select new { s.ScheduleId, s.ScheduleName, s.DateInstalled })
                    .AsEnumerable()
                    .OrderByDescending(x => x.DateInstalled)
                    .Select(x => new data.Schedule()
                    {
                        Id = x.ScheduleId ?? 0,
                        Name = x.ScheduleName,
                        LastUpdated = x.DateInstalled
                    }).FirstOrDefault();

                return ret;
            }
        }


        private data.Schedule GetCurrentSchedule(int storeId)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                var playListResult = db.Database
                    .SqlQuery<FindCurrentPlayListForStore_Result>("FindCurrentPlayListForStore")
                    .OrderBy(x => x.Importance)
                    .FirstOrDefault(x => x.StoreId == storeId);

                data.Schedule getCurrentSchedule = new data.Schedule()
                {
                    Id = playListResult?.ScheduleId ?? 0,
                    Name = playListResult?.ScheduleName,
                    LastUpdated = playListResult?.LastUpdated ?? DateTime.Now
                };
                return getCurrentSchedule;
            }
        }


        private List<data.Schedule> GetSelectedSchedules(int storeId)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                var ret = (from s in db.Schedules
                           join st in db.ScheduleStores on s.Id equals st.ScheduleID
                           where st.StoreId == storeId
                           select new { s.Id, s.Name, s.StartDate, s.EndDate, s.DefaultPlayList, s.StartTime, s.EndTime })
                    .AsEnumerable()
                    .Select(x => new data.Schedule()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        DefaultPlayList = x.DefaultPlayList,
                        StartTime = x.StartTime,
                        EndTime = x.EndTime
                    });
                return ret.ToList();
            }
        }

        private List<data.Schedule> GetAvailableSchedules(int storeId)
        {
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                var ret = (from s in db.Schedules
                           join st in db.Store on s.SignId equals st.SignId
                           where st.id == storeId
                           select new { s.Id, s.Name, s.StartDate, s.EndDate, s.DefaultPlayList, s.StartTime, s.EndTime })
                    .AsEnumerable()
                    .Select(x => new data.Schedule()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        DefaultPlayList = x.DefaultPlayList,
                        StartTime = x.StartTime,
                        EndTime = x.EndTime
                    });
                return ret.ToList();
            }
        }

        public Store Find(int storeId)
        {
            Store ret = null;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                ret = db.Store.Find(storeId);
            }
            return ret;

        }

        public bool Validate(Store entity)
        {
            ValidationErrors.Clear();
            if (!string.IsNullOrEmpty(entity.Name))
            {
                if (entity.Name.ToLower() == entity.Name)
                {
                    ValidationErrors.Add(new KeyValuePair<string, string>("Store Name", "Store Name cannot be all lower case"));
                }
            }
            return (ValidationErrors.Count == 0);
        }


        public Boolean Update(Store entity)
        {
            bool ret = false;
            if (Validate(entity))
            {
                try
                {
                    using (nightowlsign_Entities db = new nightowlsign_Entities())
                    {
                        db.Store.Attach(entity);
                        var modifiedStore = db.Entry(entity);
                        modifiedStore.Property(e => e.Name).IsModified = true;
                        modifiedStore.Property(e => e.Address).IsModified = true;
                        modifiedStore.Property(e => e.Suburb).IsModified = true;
                        modifiedStore.Property(e => e.State).IsModified = true;
                        modifiedStore.Property(e => e.Manager).IsModified = true;
                        modifiedStore.Property(e => e.Phone).IsModified = true;
                        modifiedStore.Property(e => e.SignId).IsModified = true;
                        modifiedStore.Property(e => e.IpAddress).IsModified = true;
                        modifiedStore.Property(e => e.SubMask).IsModified = true;
                        modifiedStore.Property(e => e.Port).IsModified = true;
                        db.SaveChanges();
                        ret = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException);
                    ret = false;
                }
            }
            return ret;
        }

        public Boolean Insert(Store entity)
        {
            bool ret = false;
            try
            {
                ret = Validate(entity);
                if (ret)
                {
                    using (nightowlsign_Entities db = new nightowlsign_Entities())
                    {
                        Store newStore = new Store()
                        {
                            Name = entity.Name.Trim(),
                            Address = entity.Address,
                            Suburb = entity.Suburb,
                            State = entity.State,
                            Manager = entity.Manager,
                            Phone = entity.Phone,
                            SignId = entity.SignId,
                            IpAddress = entity.IpAddress,
                            SubMask = entity.SubMask,
                            Port = entity.Port
                        };

                        db.Store.Add(newStore);
                        db.SaveChanges();
                        ret = true;
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return ret;
            }
        }


        public bool Delete(Store entity)
        {
            bool ret = false;
            using (nightowlsign_Entities db = new nightowlsign_Entities())
            {
                db.Store.Attach(entity);
                db.Store.Remove(entity);
                db.SaveChanges();
                ret = true;
            }
            return ret;
        }
    }

    public class SelectPlayList
    {
        public int Id { get; set; }
        public string PlayListName { get; set; }
        public int? StoreId { get; set; }
        public string URL { get; set; }
    }
}
