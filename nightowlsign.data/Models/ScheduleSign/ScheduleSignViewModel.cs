using nightowlsign.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nightowlsign.data.Models
{
    public class ScheduleSignViewModel : BaseModel.ViewModelBase
    {
        public ScheduleSignViewModel() : base()
        {
            schedule = new data.Schedule();
            AllSigns = new List<SelectListItem>();
        }


        public data.Schedule schedule  { get; set; }
        public List<SelectListItem> AllSigns { get; set; }
        public List<int?> schedulesign { get; set; }

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

            ScheduleSignManager sm = new ScheduleSignManager();
            schedulesign = sm.Get(schedule);
            AllSigns = sm.GetAllSigns(schedule.Id);
            foreach (SelectListItem ss in AllSigns )
            {
                ss.ScheduleId = schedule.Id;
               
                ScheduleSign selected =sm.GetValues(ss);
                if (selected != null)
                {
                    ss.selected = true;
                    ss.Id = selected.Id;
                  
                }
                //ss.selected = sm.IsSelected(ss.SignId, store.id);
                //ss.IpAddress = sm.GetIpAddress(ss.SignId, store.id);
            }
        }

    }
}
