using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using nightowlsign.data.Models.UpLoadLog;


namespace nightowlsign.data.Models.Stores
{
    public class StoreManager : IStoreManager
    {
        private data.Schedule _defaultSchedule;
        private Sign _defaultSign;
        private readonly Inightowlsign_Entities _context;
        private readonly IUpLoadLoggingManager _upLoadLoggingManager;
        public StoreManager(Inightowlsign_Entities context, IUpLoadLoggingManager upLoadLoggingManager)
        {
            _context = context;
            _upLoadLoggingManager = upLoadLoggingManager;
            ValidationErrors = new List<KeyValuePair<string, string>>();
            _defaultSchedule = new data.Schedule
            {
                Name = "No default playlist",
                StartDate = null,
                EndDate = null,
                Id = 0,
                SignId = 0
            };
            _defaultSign = new Sign
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

        public List<StoreAndSign> Get(Store entity)
        {
                var ret = _context.StoreAndSigns.OrderBy(x => x.Name).ToList<StoreAndSign>();
                if (!string.IsNullOrEmpty(entity.Name))
                {
                    ret = ret.FindAll(p => p.Name.ToLower().StartsWith(entity.Name));
                }
                GetSchedulesAndSign(ret);
                return ret;
        }

        public void GetSchedulesAndSign(List<StoreAndSign> storeList)
        {
            foreach (var store in storeList)
            {
                store.CurrentSchedule = GetCurrentSchedule(store.id);
                store.LastInstalled = GetInstalledSchedule(store.id);
                store.Sign = GetSign(store.SignId ?? 0);
            }
        }

        public Sign GetSign(int signId)
        {
                return _context.Signs.Find(signId);
        }

        public data.Schedule GetInstalledSchedule(int storeId)
        {
                var ret = (from s in _context.StoreScheduleLogs
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


        public data.Schedule GetCurrentSchedule(int storeId)
        {
                var playListResult = _context.Database
                    .SqlQuery<FindCurrentPlayListForStore_Result>("FindCurrentPlayListForStore")
                    .OrderBy(x => x.Importance)
                    .FirstOrDefault(x => x.StoreId == storeId);

                data.Schedule getCurrentSchedule = new data.Schedule()
                {
                    Id = playListResult?.ScheduleId ?? 0,
                    Name = playListResult?.ScheduleName,
                    LastUpdated = playListResult?.LastUpdated ?? DateTime.Now.ToUniversalTime()
                };
                return getCurrentSchedule;
        }


        public List<data.Schedule> GetSelectedSchedules(int storeId)
        {
                var ret = (from s in _context.Schedules
                           join st in _context.ScheduleStores on s.Id equals st.ScheduleID
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

        public List<data.Schedule> GetAvailableSchedules(int storeId)
        {
                var ret = (from s in _context.Schedules
                           join st in _context.Store on s.SignId equals st.SignId
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

        public Store Find(int storeId)
        {
                return _context.Store.Find(storeId);
        }

        public bool Validate(Store entity)
        {
            ValidationErrors.Clear();
            if (string.IsNullOrEmpty(entity.Name)) return (ValidationErrors.Count == 0);
            if (entity.Name.ToLower() == entity.Name)
            {
                ValidationErrors.Add(new KeyValuePair<string, string>("Store Name", "Store Name cannot be all lower case"));
            }
            return (ValidationErrors.Count == 0);
        }

        public bool Update(StoreAndSign storeAndSign, int successCode)
        {
            var store = Find(storeAndSign.id);
            store.LastUpdateStatus =
                _upLoadLoggingManager.GetOverallStatus(storeAndSign.id, storeAndSign.CurrentSchedule.Id);
            store.LastUpdateTime = DateTime.Now.ToUniversalTime();
            return Update(store);
        }

        public bool Update(Store entity)
        {
            if (!Validate(entity)) return false;
            try
            {
                   _context.Store.Attach(entity);
                    var modifiedStore = _context.Entry(entity);
                    modifiedStore.Property("Name").IsModified = true;
                    modifiedStore.Property("Address").IsModified = true;
                    modifiedStore.Property("Suburb").IsModified = true;
                    modifiedStore.Property("State").IsModified = true;
                    modifiedStore.Property("Manager").IsModified = true;
                    modifiedStore.Property("Phone").IsModified = true;
                    modifiedStore.Property("SignId").IsModified = true;
                    modifiedStore.Property("IpAddress").IsModified = true;
                    modifiedStore.Property("SubMask").IsModified = true;
                    modifiedStore.Property("Port").IsModified = true;
                    modifiedStore.Property("ProgramFile").IsModified = true;
                    modifiedStore.Property("LastUpdateTime").IsModified = true;
                    modifiedStore.Property("LastUpdateStatus").IsModified = true;
                _context.SaveChanges();
                    return true;
 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return false;
            }
        }

        public bool Insert(Store entity)
        {
            bool ret = false;
            try
            {
                ret = Validate(entity);
                if (Validate(entity))
                {
                        var newStore = new Store()
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
                            Port = entity.Port,
                            ProgramFile = entity.ProgramFile
                        };

                        _context.Store.Add(newStore);
                        _context.SaveChanges();
                        ret = true;
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
            try
            {
                    _context.Store.Attach(entity);
                    _context.Store.Remove(entity);
                    _context.SaveChanges();
                    return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
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
