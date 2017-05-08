using nightowlsign.data;
using nightowlsign.data.Models.Stores;

namespace NightOwlImageService.Services
{
    public interface IServiceRunner
    {
        void Start();
        void Stop();
        void DoTheWork(IStoreViewModel storeViewModel);
        void UpdateTheDataBase(StoreAndSign storeAndSign, int successCode);
        int SendTheScheduleToSign(StoreAndSign storeAndSign);
    }
}