using System.Collections.Generic;


namespace nightowlsign.data.Models
{
    public class ScheduleImageViewModel : BaseModel.ViewModelBase
    {
        public ScheduleImageViewModel() : base()
        {
            Schedule = new data.Schedule();
            AllImages = new List<ImageSelect>();
        }


        public data.Schedule Schedule { get; set; }
        public int SignId { get; set; }
        public List<ImageSelect> AllImages { get; set; }
        public List<int?> scheduleImage { get; set; }

        protected override void Init()
        {
            base.Init();
        }

        public override void HandleRequest()
        {
            base.HandleRequest();
        }

        public void LoadData()
        {
            Get();
        }

        protected override void Get()
        {
            ScheduleImageManager sm = new ScheduleImageManager();
            AllImages = sm.GetAllImages(SignId, Schedule.Id);
            foreach (ImageSelect imageSelect in AllImages)
            {
                imageSelect.ScheduleId = Schedule.Id;
                ScheduleImage selected = sm.GetValues(imageSelect);
                if (selected != null)
                {
                    imageSelect.Selected = true;
                    imageSelect.Id = selected.Id;
                    imageSelect.ImageId = selected.ImageID ?? 0;
                    imageSelect.SignId = SignId;
                }
            }
        }
    }
}
