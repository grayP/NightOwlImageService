using System;
using System.Timers;
using Autofac;
using ConfigInjector;
using ImageProcessor.Services;
using Logger;
using nightowlsign.data;
using nightowlsign.data.Models.Stores;
using nightowlsign.data.Models.StoreScheduleLog;
using NightOwlImageService.Configuration;
using Serilog;

namespace NightOwlImageService.Services
{
    public class ServiceRunner
    {
        readonly Timer _timer;
        private mLogger _logger;
        private RunnerCycleTime runCycleTime;
        public void Start() { _timer.Start(); }
        public void Stop() { _timer.Stop(); }


        public ServiceRunner()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<RunnerCycleTime>().As<ConfigInjector.IConfigurationSetting>();

            _logger = new mLogger();
            _timer = new Timer { AutoReset = true };
            _timer.Interval = 1800000;
            _timer.Elapsed += (sender, eventArgs) => DoTheWork();
        }

        public void DoTheWork()
        {
            StoreViewModel svm = new StoreViewModel { EventCommand = "List" };
            svm.HandleRequest();
            foreach (StoreAndSign storeAndSign in svm.StoresAndSigns)
            {
                if (storeAndSign?.CurrentSchedule.Id != storeAndSign?.LastInstalled?.Id && storeAndSign.CurrentSchedule.Id != 0)
                {
                    Console.WriteLine($"Starting on store {storeAndSign.Name} - {DateTime.Now}");
                    _logger.WriteLog($"Starting on store {storeAndSign.Name} - {DateTime.Now}");
                    Log.Information($"Starting on store {storeAndSign.Name} - {DateTime.Now}");
                    if (SendTheScheduleToSign(storeAndSign, _logger))
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
                        if (sslm.Insert())
                        {
                            _logger.WriteLog($"Updated {storeAndSign.id}-{DateTime.Now}");
                        }
                        else
                        {
                            _logger.WriteLog($"sslm.errorMessage -{DateTime.Now}");
                        }
                    };
                    this._logger.WriteLog(
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
