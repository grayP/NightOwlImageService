using System.Collections.Generic;

namespace nightowlsign.data.Models.Stores
{
    public interface IStoreViewModel
    {
        List<Store> Stores { get; set; }
        List<StoreAndSign> StoresAndSigns { get; set; }
        Store SearchEntity { get; set; }
        Store Entity { get; set; }
        IEnumerable<SelectListItem> SignList { get; }
        string Mode { get; set; }
        string EventCommand { get; set; }
        string EventArgument { get; set; }
        bool IsDetailVisible { get; set; }
        bool IsSearchVisible { get; set; }
        bool IsListAreaVisible { get; set; }
        bool IsValid { get; set; }
        List<KeyValuePair<string, string>> ValidationErrors { get; set; }
        void HandleRequest();
    }
}