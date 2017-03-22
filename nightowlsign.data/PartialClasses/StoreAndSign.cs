using System;
using System.Collections.Generic;

namespace nightowlsign.data
{
   public partial class StoreAndSign
    {
        //public List<SelectPlayList> PlayLists { get; set; }
        public Schedule CurrentSchedule { get; set; }
        public Schedule LastInstalled { get; set; }
        public List<Schedule> AvailableSchedules { get; set; }
        public List<Schedule> SelectedSchedules { get; set; }
        public Sign Sign { get; set; }

        public void GetPlayLists()
        {
            throw new NotImplementedException();
        }
        public bool AddressOk()
        {
            return IpAddress != null && SubMask != null && Port != null;
        }
    }

}
