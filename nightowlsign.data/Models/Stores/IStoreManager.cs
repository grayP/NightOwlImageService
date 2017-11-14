using System.Collections.Generic;

namespace nightowlsign.data.Models.Stores
{
    public interface IStoreManager
    {
        List<KeyValuePair<string, string>> ValidationErrors { get; set; }
        List<StoreAndSign> Get(Store entity);
        void GetSchedulesAndSign(List<StoreAndSign> storeList);
        Sign GetSign(int signId);
        data.Schedule GetInstalledSchedule(int storeId);
        data.Schedule GetCurrentSchedule(int storeId);
        List<data.Schedule> GetSelectedSchedules(int storeId);
        List<data.Schedule> GetAvailableSchedules(int storeId);
        Store Find(int storeId);
        bool Validate(Store entity);
        bool Update(StoreAndSign storeAndSign);
        bool Update(Store entity);
        void CleanOutOldSchedule(StoreAndSign storeAndSign);
    }
}