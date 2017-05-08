using System;
using System.Collections.Generic;
using nightowlsign.data.Models.SendToSign;
using nightowlsign.data.Models.Signs;
using nightowlsign.data.Models.StoreSignDto;


namespace nightowlsign.data.Models
{
    public class SendToSignViewModel : BaseModel.ViewModelBase
    {
        private readonly ISendToSignManager _sendToSignManager;
        public SendToSignViewModel(ISendToSignManager sendToSignManager) : base()
        {
            _sendToSignManager = sendToSignManager;
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
            SignsForSchedule = _sendToSignManager.GetSignsForSchedule(Schedule.Id);
            AllImagesInSchedule = _sendToSignManager.GetImagesForThisSchedule(Schedule.Id);
            StoresForSchedule = _sendToSignManager.GetStoresWithThisSign(Schedule.Id);
        }
    }
}
