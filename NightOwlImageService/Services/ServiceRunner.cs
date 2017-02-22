using System;
using System.Timers;
using ImageProcessor.Services;
using nightowlsign.data;
using nightowlsign.data.Models;
using nightowlsign.data.Models.Stores;

namespace NightOwlImageService.Services
{
    public class ServiceRunner
    {
        readonly Timer _timer;
        //  var RssRead = new ReadWeatherFeed();
        public void Start() { _timer.Start(); }
        public void Stop() { _timer.Stop(); }


        public ServiceRunner()
        {

            //_timer = new Timer { AutoReset = true };
           // _timer.Interval = 5000;
           // _timer.Elapsed += (sender, eventArgs) => DoTheWork();
            DoTheWork();
        }

        public void DoTheWork()
        {

            StoreViewModel svm = new StoreViewModel {EventCommand = "List"};
            svm.HandleRequest();
            foreach (StoreAndSign storeAndSign in svm.StoresAndSigns)
            {
                if (storeAndSign?.CurrentSchedule.Id != storeAndSign?.LastInstalled?.Id && storeAndSign.CurrentSchedule.Id != 0)
                {
                    SendTheScheduleToSign(storeAndSign);
                    Console.WriteLine(
                        $"Uploaded images for {storeAndSign.Name} store, schedule: {storeAndSign.CurrentSchedule.Name}");
                }
            }
           
        }

        private void SendTheScheduleToSign(StoreAndSign storeAndSign)
        {
            CreateFilesToSend createFilesToSend = new CreateFilesToSend(storeAndSign);
            createFilesToSend.Run();
        }
    }
}
