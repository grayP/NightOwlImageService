using nightowlsign.data.Models.Signs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nightowlsign.data.Models.Schedule
{
    public class ScheduleViewModel : BaseModel.ViewModelBase
    {
        private readonly ScheduleManager _scheduleManager;

        public ScheduleViewModel() : base()
        {
            _scheduleManager = new ScheduleManager();

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
            Schedules = _scheduleManager.Get(SearchEntity);
        }

        protected override void Edit()
        {
            Entity = _scheduleManager.Find(Convert.ToInt32(EventArgument));
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
            if (Mode == "Add")
            {
                _scheduleManager.Insert(Entity);
            }
            else
            {
                _scheduleManager.Update(Entity);
            }
            ValidationErrors = _scheduleManager.ValidationErrors;
            base.Save();
        }

        protected override void Delete()
        {
            Entity = _scheduleManager.Find(Convert.ToInt32(EventArgument));
            _scheduleManager.Delete(Entity);
            Get();
            base.Delete();
        }
    }
}
