using nightowlsign.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nightowlsign.data.Models.Stores
{
    public class StoreViewModel : BaseModel.ViewModelBase
    {
        public StoreViewModel() : base()
        {
        }
        public List<Store> Stores { get; set; }
        public List<StoreAndSign> StoresAndSigns { get; set; }
        public Store SearchEntity { get; set; }
        public Store Entity { get; set; }
        public IEnumerable<SelectListItem> SignList
        {
            get
            {
                using (nightowlsign_Entities db = new nightowlsign_Entities())
                {
                    var selectList = new List<SelectListItem>()
                    {
                        new SelectListItem
                        {
                            Id = 0,
                            Model = "Show All"
                        }
                    };
                    selectList.AddRange(from item in
                                      db.Signs.OrderBy(x => x.Model)
                                        select new SelectListItem()
                                        {
                                            SignId = item.id,
                                            Model = item.Model
                                        });

                    return selectList;
                }
            }
        }


        protected override void Init()
        {
            Stores = new List<Store>();
            SearchEntity = new Store();
            Entity = new Store();
            base.Init();
        }
        public override void HandleRequest()
        {
            base.HandleRequest();
        }
        protected override void ResetSearch()
        {
            SearchEntity = new Store();
        }
        protected override void Get()
        {
            StoreManager sm = new StoreManager();
            StoresAndSigns = sm.Get(SearchEntity);

        }
        protected override void Edit()
        {
            StoreManager sm = new StoreManager();
            Entity = sm.Find(Convert.ToInt32(EventArgument));
            base.Edit();
        }
        protected override void Add()
        {
            IsValid = true;
            Entity = new Store();
            Entity.Name = "";
            base.Add();
        }
        protected override void Save()
        {
            StoreManager sm = new StoreManager();
            if (Mode == "Add")
            {
                sm.Insert(Entity);
            }
            else
            {
                sm.Update(Entity);
            }
            ValidationErrors = sm.ValidationErrors;
            base.Save();
        }
        protected override void Delete()
        {
            StoreManager sm = new StoreManager();
            Entity = sm.Find(Convert.ToInt32(EventArgument));
            sm.Delete(Entity);
            Get();
            base.Delete();
        }
    }
}
