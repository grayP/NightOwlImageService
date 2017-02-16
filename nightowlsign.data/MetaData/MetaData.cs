using System;
using System.ComponentModel.DataAnnotations;

namespace nightowlsign.data
{
    public class ScheduleMetaData
    {
        //[Range(typeof(DateTime),"1/1/2016","31/12/2050", ErrorMessage = "Date must be between 2016 and 2050")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? StartDate { get; set; }


       // [Range(typeof(DateTime), "1/1/2016", "31/12/2050", ErrorMessage = "Date must be between 2016 and 2050")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? EndDate { get; set; }

    }
}
