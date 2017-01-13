using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace NightOwlImageService.Services
{
   public class SendImageToSign
    {
        readonly Timer _timer;
        public SendImageToSign()
        {
            _timer = new Timer { AutoReset = true };
            _timer.Elapsed += (sender, eventArgs) => Console.WriteLine("It is {0} and all is well", DateTime.Now);
        }
        public void Start() { _timer.Start(); }
        public void Stop() { _timer.Stop(); }
    }
}
