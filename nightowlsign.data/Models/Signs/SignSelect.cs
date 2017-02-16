using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nightowlsign.data.Models
{
    public class SelectListItem
    {
        public bool selected { get; set; }
        public int Id { get; set; }
        public int? StoreId { get; set; }
        public int? SignId { get; set; }
        public string Model { get; set; }
        public string IpAddress { get; set; }
        public string SubMask { get; set; }
        public string Port { get; set; }
        public int? ScheduleId { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public decimal? AspectRatio { get; set; }

    }
}
