using nightowlsign.data.Models.ScreenBrightness;
using System.Runtime.CompilerServices;
using System;
namespace nightowlsign.data
{
    public partial class StoreAndSign
    {
        public int NumImagesUploaded { get; set; }
        public bool SignNeedsToBeUpdated()
        {
            return false;
            return this?.CurrentSchedule.Id != this?.LastInstalled?.Id && this.CurrentSchedule?.Id != 0 ||
                   this?.CurrentSchedule.Id == this?.LastInstalled?.Id && this.LastUpdateStatus < 0 ||
                   this?.CurrentSchedule?.LastUpdated > this?.LastInstalled?.LastUpdated && this.CurrentSchedule?.Id != 0;
        }

        public bool CheckForChangeInSchedule()
        {
            if (this.LastInstalled == null) return false;
            if (this.CurrentSchedule.Id != this.LastInstalled?.Id || this.LastUpdateStatus == -99)
            {
                return true;
            }
            return false;
        }

        public bool BrightnessNeedsToBeSet(ScreenBrightnessManager sbm, int storeId)
        {
            return sbm.SignBrightnessNeedsToBeSet(storeId, DateTime.Now.Minute);
        }
    }
}
