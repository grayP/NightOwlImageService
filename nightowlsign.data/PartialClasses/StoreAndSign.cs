namespace nightowlsign.data
{
    public partial class StoreAndSign
    {
        public bool SignNeedsToBeUpdated ()
        {
            return this?.CurrentSchedule.Id != this?.LastInstalled?.Id && this.CurrentSchedule?.Id != 0 ||
                   this?.CurrentSchedule.LastUpdated > this?.LastInstalled?.LastUpdated && this.CurrentSchedule?.Id != 0;
        }
    }
}
