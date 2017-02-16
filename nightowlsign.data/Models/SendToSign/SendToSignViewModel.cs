using System;
using System.Collections.Generic;
using nightowlsign.data.Models.Signs;
using nightowlsign.data.Models.StoreSignDto;


namespace nightowlsign.data.Models
{
    public class SendToSignViewModel : BaseModel.ViewModelBase
    {
        public SendToSignViewModel() : base()
        {
            DisplayMessage = "";
            DebugMessage = "";
            Schedule = new data.Schedule();
            AllImagesInSchedule = new List<ImageSelect>();
        }


        public data.Schedule Schedule { get; set; }
        public List<ImageSelect> AllImagesInSchedule { get; set; }
        public List<SignDto> SignsForSchedule { get; set; }
        public List<StoreSignDTO> StoresForSchedule { get; set; }
        public SignParameters SignParameter { get; set; }

        public string DisplayMessage { get; set; }
        public string DebugMessage { get; set; }
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
            SendToSignManager sm = new SendToSignManager();
            SignsForSchedule = sm.GetSignsForSchedule(Schedule.Id);
            AllImagesInSchedule = sm.GetImagesForThisSchedule(Schedule.Id);
            StoresForSchedule = sm.GetStoresWithThisSign(Schedule.Id);
        }
    }
}
