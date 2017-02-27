using System;
using System.Timers;
using ImageProcessor.Services;
using Logger;
using nightowlsign.data;
using nightowlsign.data.Models;
using nightowlsign.data.Models.Stores;
using nightowlsign.data.Models.StoreScheduleLog;

namespace NightOwlImageService.Services
{
    public class ServiceRunner
    {
        readonly Timer _timer;
        private mLogger Logger;
        public void Start() { _timer.Start(); }
        public void Stop() { _timer.Stop(); }


        public ServiceRunner()
        {
            Logger= new mLogger();
            _timer = new Timer { AutoReset = true };
            _timer.Interval = 5000;
            _timer.Elapsed += (sender, eventArgs) => DoTheWork();
            //DoTheWork();
        }

        public void DoTheWork()
        {
            StoreViewModel svm = new StoreViewModel {EventCommand = "List"};
            svm.HandleRequest();
            foreach (StoreAndSign storeAndSign in svm.StoresAndSigns)
            {
                if (storeAndSign?.CurrentSchedule.Id != storeAndSign?.LastInstalled?.Id && storeAndSign.CurrentSchedule.Id != 0)
                {
                    if (SendTheScheduleToSign(storeAndSign, Logger))
                    {
                        StoreScheduleLogManager sslm = new StoreScheduleLogManager()
                        {
                            entity = new StoreScheduleLog()
                            {
                                DateInstalled = DateTime.Now,
                                ScheduleName = storeAndSign.CurrentSchedule.Name,
                                ScheduleId = storeAndSign.CurrentSchedule.Id,
                                InstalledOk = true,
                                StoreId = storeAndSign.id
                            }
                        };
                        sslm.Insert();
                    };
                    this.Logger.WriteLog(
                        $"ServiceRunner - doTheWork - Uploaded images for {storeAndSign.Name} store, schedule: {storeAndSign.CurrentSchedule.Name}");
                }
            }
        }


        private bool SendTheScheduleToSign(StoreAndSign storeAndSign, mLogger logger)
        {
            CreateFilesToSend createFilesToSend = new CreateFilesToSend(storeAndSign, logger);
            return createFilesToSend.Run();
         }
    }
}
