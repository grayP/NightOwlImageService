using System;
using System.Net.Mime;
using System.Timers;
using Autofac;
using ConfigInjector;
using ImageProcessor.Services;
using Logger;
using nightowlsign.data;
using nightowlsign.data.Models.Stores;
using nightowlsign.data.Models.StoreScheduleLog;
using NightOwlImageService.Configuration;

namespace NightOwlImageService.Services
{
    public class ServiceRunner
    {
        readonly Timer _timer;
        private MLogger _logger;
        private RunnerCycleTime runCycleTime;
        private System.Reflection.Assembly _assembly;

        public void Start()
        {
            DoTheWork();
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }


        public ServiceRunner(System.Reflection.Assembly assembly)
        {
            _assembly = assembly;
            var builder = new ContainerBuilder();
           // builder.RegisterType<RunnerCycleTime>().As<ConfigInjector.IConfigurationSetting>();

            _logger = new MLogger(_assembly.FullName);
            _timer = new Timer { AutoReset = true };
            _timer.Interval = 120000;
            _timer.Elapsed += (sender, eventArgs) => DoTheWork();
        }

        public void DoTheWork()
        {
            StoreViewModel svm = new StoreViewModel { EventCommand = "List" };
            svm.HandleRequest();
            foreach (StoreAndSign storeAndSign in svm.StoresAndSigns)
            {
                if (storeAndSign?.CurrentSchedule.Id != storeAndSign?.LastInstalled?.Id && storeAndSign.CurrentSchedule.Id != 0 || storeAndSign?.CurrentSchedule.LastUpdated>storeAndSign?.LastInstalled?.LastUpdated)
                {
                    Console.WriteLine($"Starting on store {storeAndSign.Name} ");
                    _logger.WriteLog($"Starting on store {storeAndSign.Name} ");
                    if (SendTheScheduleToSign(storeAndSign, _logger))
                    {

                        StoreScheduleLogManager sslm = new StoreScheduleLogManager()
                        {
                            Entity = new StoreScheduleLog()
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
                            _logger.WriteLog($"Updated {storeAndSign.id}");
                        }
                        else
                        {
                            _logger.WriteLog($"{sslm.ErrorMessage} ");
                        }
                    };
                    this._logger.WriteLog(
                        $"ServiceRunner - doTheWork - Uploaded images for {storeAndSign.Name} store, schedule: {storeAndSign.CurrentSchedule.Name}");
                }
            }
            CheckIfTimeToClose();
        }

        private void CheckIfTimeToClose()
        {
            var TimeNow = DateTime.Now.Hour;
            if (TimeNow >= 23)
            {
                Environment.Exit(0);
            }
        }

        private bool SendTheScheduleToSign(StoreAndSign storeAndSign, MLogger logger)
        {
            CreateFilesToSend createFilesToSend = new CreateFilesToSend(storeAndSign, logger);
            return createFilesToSend.Run();
        }
    }
}
