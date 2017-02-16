using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nightowlsign.data.Models.Stores;

namespace nightowlsign.data
{
   public partial class StoreAndSign
    {
        //public List<SelectPlayList> PlayLists { get; set; }
        public Schedule CurrentSchedule { get; set; }
        public List<Schedule> AvailableSchedules { get; set; }
        public List<Schedule> SelectedSchedules { get; set; }
        public Sign Sign { get; set; }

        public void GetPlayLists()
        {
            throw new NotImplementedException();
        }
    }

}
