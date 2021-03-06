using nightowlsign.data;
using nightowlsign.data.Models.Stores;

namespace NightOwlImageService.Services
{
    public interface IServiceRunner
    {
        void Start();
        void Stop();
        void DoTheWork();
        void UpdateTheDataBase(StoreAndSign storeAndSign, IStoreManager storeManager);
        void SendTheScheduleToSign(StoreAndSign storeAndSign);
    }
}