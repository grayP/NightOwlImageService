using nightowlsign.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nightowlsign.data.Models
{
    public class StoreSignIPViewModel : BaseModel.ViewModelBase
    {
        public StoreSignIPViewModel() : base()
        {
            store = new Store();
            AllSigns = new List<SignSelect>();
        }


        public Store store { get; set; }
        public List<SignSelect> AllSigns { get; set; }
        public List<int?> storesign { get; set; }

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
            AllSigns = sm.GetAllSigns();
            foreach (SignSelect ss in AllSigns )
            {
                ss.selected = sm.IsSelected(ss.Id, store.id);
            }
        }

    }
}
