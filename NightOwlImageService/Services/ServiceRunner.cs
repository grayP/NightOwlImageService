using System;
using System.Net.Mime;
using System.Timers;
using Autofac;
using ConfigInjector;
using ImageProcessor.Services;
using Logger;
using nightowlsign.data;
using nightowlsign.data.Models.SendToSign;
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
        private ISendCommunicator _sendCommunicator;
        private IStoreScheduleLogManager _storeScheduleLogManager;
        private ICreateFilesToSend _createFilesToSend;
        private ISendToSignManager _sendToSignManager;




        public void Start()
        {
            DoTheWork();
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }


        public ServiceRunner(ISendCommunicator sendCommunicator, ICreateFilesToSend createFilesToSend, IStoreScheduleLogManager storeScheduleLogManager, ISendToSignManager sendToSignManager)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
          
            _logger = new MLogger(assembly.FullName);
            _sendCommunicator = sendCommunicator;
            _storeScheduleLogManager = storeScheduleLogManager;
            _createFilesToSend = createFilesToSend;
            _sendToSignManager = sendToSignManager;

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
                        if (SendTheScheduleToSign(storeAndSign, _logger, _sendCommunicator, _sendToSignManager))
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
            var timeNow = DateTime.Now.Hour;
            if (timeNow >= 23)
            {
                Environment.Exit(0);
            }
        }

        private bool SendTheScheduleToSign(StoreAndSign storeAndSign, MLogger logger, ISendCommunicator sendCommunicator, ISendToSignManager sendToSignManager)
        {
            _createFilesToSend.Init(storeAndSign, logger, sendCommunicator, sendToSignManager);
            return _createFilesToSend.Run();
        }
    }
}
