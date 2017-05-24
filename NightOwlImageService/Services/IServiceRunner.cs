using nightowlsign.data;
using nightowlsign.data.Models.Stores;

namespace NightOwlImageService.Services
{
    public interface IServiceRunner
    {
        void Start();
        void Stop();
        void DoTheWork(Inightowlsign_Entities context);
        void UpdateTheDataBase(StoreAndSign storeAndSign, int successCode, IStoreManager storeManager);
        int SendTheScheduleToSign(StoreAndSign storeAndSign);
    }
}