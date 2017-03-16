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
      //  private System.Reflection.Assembly _assembly;
        private readonly SendCommunicator _sendCommunicator;
        private readonly StoreScheduleLogManager _storeScheduleLogManager;
        private CreateFilesToSend _createFilesToSend;




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
           
            var builder = new ContainerBuilder();
            // builder.RegisterType<RunnerCycleTime>().As<ConfigInjector.IConfigurationSetting>();

            _logger = new MLogger(assembly.FullName);
            _sendCommunicator = new SendCommunicator();
            _storeScheduleLogManager = new StoreScheduleLogManager();
            _createFilesToSend = new CreateFilesToSend();

            _timer = new Timer { AutoReset = true };
            _timer.Interval = 300000;
            _timer.Elapsed += (sender, eventArgs) => DoTheWork();
        }

        public void DoTheWork()
        {
            StoreViewModel svm = new StoreViewModel { EventCommand = "List" };
            svm.HandleRequest();
            foreach (StoreAndSign storeAndSign in svm.StoresAndSigns)
            {
                Console.WriteLine($"Checking store {storeAndSign.Name}");
                _logger.WriteLog($"Checking store: {storeAndSign.Name}");
                if (storeAndSign.IpAddress != null)
                {
                    storeAndSign.CurrentSchedule = svm.GetCurrentSchedule(storeAndSign);
                    if (storeAndSign?.CurrentSchedule.Id != storeAndSign?.LastInstalled?.Id &&
                        storeAndSign.CurrentSchedule.Id != 0 ||
                        storeAndSign?.CurrentSchedule.LastUpdated > storeAndSign?.LastInstalled?.LastUpdated)
                    {
                        Console.WriteLine($"Starting on store {storeAndSign.Name} ");
                        _logger.WriteLog($"Starting on store {storeAndSign.Name} ");
                        if (SendTheScheduleToSign(storeAndSign, _logger))
                        {
                            _storeScheduleLogManager.SetValues(storeAndSign.CurrentSchedule.Name, storeAndSign.CurrentSchedule.Id, storeAndSign.id);
                            _logger.WriteLog(_storeScheduleLogManager.Insert() ? $"Updated {storeAndSign.id}" : $"{_storeScheduleLogManager.ErrorMessage} ");
                        }
                        ;
                        this._logger.WriteLog(
                            $"ServiceRunner - doTheWork - Uploaded images for {storeAndSign.Name} store, schedule: {storeAndSign.CurrentSchedule.Name}");
                    }
                }
                else
                {
                    _logger.WriteLog($" Null IP Address for {storeAndSign.Name}");
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
            _createFilesToSend.Init(storeAndSign, logger, _sendCommunicator);
            return _createFilesToSend.Run();
        }
    }
}
