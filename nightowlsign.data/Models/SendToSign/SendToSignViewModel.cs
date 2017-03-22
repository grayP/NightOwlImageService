﻿using nightowlsign.data.Models.Image;
using nightowlsign.data.Models.Signs;
using System.Collections.Generic;
  

namespace nightowlsign.data.Models.SendToSign
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
        public List<StoreSignDto> StoresForSchedule { get; set; }
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
