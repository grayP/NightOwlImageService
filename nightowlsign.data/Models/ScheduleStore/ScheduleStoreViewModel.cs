using System.Collections.Generic;

namespace nightowlsign.data.Models.ScheduleStore
{
    public class ScheduleStoreViewModel : BaseModel.ViewModelBase
    {
        public ScheduleStoreViewModel() : base()
        {
            Schedule = new data.Schedule();
            AllStores = new List<StoreSelect>();
        }


        public data.Schedule Schedule { get; set; }
        public List<StoreSelect> AllStores { get; set; }
        public List<int?> schedulestore { get; set; }
        public int SignSize { get; set; }
    

        protected override void Init()
        {

            base.Init();
        }

        public override void HandleRequest()
        {
            base.HandleRequest();
        }

        public void LoadData(int signId)
        {
            SignSize = signId;
            Get();
        }

        protected override void Get()
        {

            ScheduleStoreManager sm = new ScheduleStoreManager();
         //   schedulestore = sm.Get(Schedule);
            AllStores = sm.GetAllStores(SignSize);
            foreach (StoreSelect ss in AllStores)
            {
                ss.ScheduleId = Schedule.Id;
                data.ScheduleStore selected = sm.GetValues(ss);
                if (selected != null)
                {
                    ss.Selected = true;
                    ss.Id = selected.Id;
                    ss.StoreId = selected.StoreId ?? 0;
                }
               // ss.Selected = sm.IsSelected(Schedule.Id, ss.Id);
            }
        }
    }
}
