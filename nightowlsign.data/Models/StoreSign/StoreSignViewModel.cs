using nightowlsign.data.Models.Signs;
using System.Collections.Generic;


namespace nightowlsign.data.Models.StoreSign
{
    public class StoreSignViewModel : BaseModel.ViewModelBase
    {
        public StoreSignViewModel()
        {
            
        }
        public StoreSignViewModel(int id, string storeName) : base()
        {
            store = new Store
            {
                id = id,
                Name = storeName
            };
            AllSigns = new List<SelectListItem>();
        }


        public Store store { get; set; }
        public IEnumerable<SelectListItem> AllSigns { get; set; }
        public IEnumerable<int?> storesign { get; set; }

        protected override void Init()
        {

            base.Init();
        }

        public override void HandleRequest()
        {
            base.HandleRequest();
        }

        public void loadData()
        {
            Get();
        }

        protected override void Get()
        {
            StoreSignManager sm = new StoreSignManager();
            storesign = sm.Get(store);
            AllSigns = sm.GetAllSigns(store.id);
            foreach (SelectListItem ss in AllSigns )
            {
               data.StoreSign selected=sm.GetValues(ss);
                if (selected != null)
                {
                    ss.selected = true;
                    ss.Id = selected.id;
                    ss.IpAddress = selected.IPAddress;
                    ss.SubMask = selected.SubMask;
                    ss.Port = selected.Port;
                }
            }
        }

    }
}
