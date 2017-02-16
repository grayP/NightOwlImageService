using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nightowlsign.data.Models.Schedule
{
    public class ScheduleViewModel : BaseModel.ViewModelBase
    {

        public ScheduleViewModel() : base()
        {

        }
        public List<data.ScheduleAndSign> Schedules { get; set; }
        public data.Schedule SearchEntity { get; set; }
        public data.Schedule Entity { get; set; }
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
            Schedules = new List<data.ScheduleAndSign>();
            SearchEntity = new data.Schedule();
            Entity = new data.Schedule();
            base.Init();
        }
        public override void HandleRequest()
        {
            base.HandleRequest();
        }
        protected override void ResetSearch()
        {
            SearchEntity = new data.Schedule();
        }

        protected override void Get()
        {
            ScheduleManager sm = new ScheduleManager();
            Schedules = sm.Get(SearchEntity);
        }

        protected override void Edit()
        {
            ScheduleManager sm = new ScheduleManager();
            Entity = sm.Find(Convert.ToInt32(EventArgument));
            base.Edit();
        }

        protected override void Add()
        {
            IsValid = true;
            Entity = new data.Schedule();
            base.Add();
        }
        protected override void Save()
        {
            ScheduleManager sm = new ScheduleManager();
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
            ScheduleManager sm = new ScheduleManager();
            Entity = sm.Find(Convert.ToInt32(EventArgument));
            sm.Delete(Entity);
            Get();
            base.Delete();
        }
    }
}
