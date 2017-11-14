using nightowlsign.data.Models.ScreenBrightness;

namespace nightowlsign.data
{
    public partial class StoreAndSign
    {
        public int NumImagesUploaded { get; set; }
        public int SuccessCode { get; set; }
        public bool SignNeedsToBeUpdated()
        {

            if (this?.LastUpdateStatus == 1  || this.CurrentSchedule.Id == 0)
            {
                //Last Update Status= 1 means store has been turned off!
                return false;
            }
            else
            {
                return this?.CurrentSchedule.Id != this?.LastInstalled?.Id ||
                       this?.CurrentSchedule.Id == this?.LastInstalled?.Id && this.LastUpdateStatus < 0 ||
                       this?.CurrentSchedule?.LastUpdated > this?.LastInstalled?.LastUpdated  ||
                       this?.LastUpdateStatus == -99;
            }
        }

        public bool CheckForChangeInSchedule()
        {
           return this.LastInstalled == null ?  false : this.CurrentSchedule.Id != this.LastInstalled?.Id || this.LastUpdateStatus == -99;
        }

        public bool BrightnessNeedsToBeSet(ScreenBrightnessManager sbm, int storeId)
        {
            return true;
            // return sbm.SignBrightnessNeedsToBeSet(storeId, DateTime.Now.Minute);
        }
    }
}
