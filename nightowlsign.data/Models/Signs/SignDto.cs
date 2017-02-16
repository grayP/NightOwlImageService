using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nightowlsign.data.Models.Signs
{
    public class SignDto
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string IPAddress { get; set; }
        public int StoreId { get; set; }
    }
}
