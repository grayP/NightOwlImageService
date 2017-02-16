using System;
using System.Timers;

namespace NightOwlImageService.Services
{
    public class ServiceRunner
    {
        readonly Timer _timer;
        //  var RssRead = new ReadWeatherFeed();
        public void Start() { _timer.Start(); }
        public void Stop() { _timer.Stop(); }

        SendImageToSign _sendImageToSign;


        public ServiceRunner()
        {

            _sendImageToSign = new SendImageToSign();
            _timer = new Timer { AutoReset = true };
            _timer.Interval = 5000;
            _timer.Elapsed += (sender, eventArgs) =>DoTheWork();


        }

        public void  DoTheWork()
        {
            Console.WriteLine("The work is done");

        }

    }
}
