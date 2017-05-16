namespace nightowlsign.data
{
    public partial class StoreAndSign
    {
        public int NumImagesUploaded  { get; set; }
        public bool SignNeedsToBeUpdated ()
        {
           
            return this?.CurrentSchedule.Id != this?.LastInstalled?.Id && this.CurrentSchedule?.Id != 0 ||
                   this?.CurrentSchedule.Id == this?.LastInstalled?.Id && this.LastUpdateStatus < 0 ||
                   this?.CurrentSchedule?.LastUpdated > this?.LastInstalled?.LastUpdated && this.CurrentSchedule?.Id != 0;
        }
    }
}
