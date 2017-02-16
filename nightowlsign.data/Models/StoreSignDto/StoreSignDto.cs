using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nightowlsign.data.Models.StoreSignDto
{
    public class StoreSignDTO
    {
        public int Id { get; set; }
        public string StoreName { get; set; }
        public string IPAddress { get; set; }
        public string SubMask { get; set; }
        public string Port { get; set; }
        public int StoreId { get; set; }
    }
}
