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
        private IMLogger _logger;
        private RunnerCycleTime runCycleTime;
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


        public ServiceRunner(ISendCommunicator sendCommunicator, ICreateFilesToSend createFilesToSend, IStoreScheduleLogManager storeScheduleLogManager, ISendToSignManager sendToSignManager, IMLogger logger)
        {
            _logger = logger;
            _sendCommunicator = sendCommunicator;
            _storeScheduleLogManager = storeScheduleLogManager;
            _createFilesToSend = createFilesToSend;
            _sendToSignManager = sendToSignManager;

            _logger.init(System.Reflection.Assembly.GetExecutingAssembly().FullName);

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
                _logger.WriteLog($"Checking store: {storeAndSign.Name}");
                if (storeAndSign.IpAddress != null)
                {
                    storeAndSign.CurrentSchedule = svm.GetCurrentSchedule(storeAndSign);
                    _logger.WriteLog($"Current: {storeAndSign?.CurrentSchedule.Name}, LastInstalled: {storeAndSign?.LastInstalled.Name}");
                    if (storeAndSign?.CurrentSchedule.Id != storeAndSign?.LastInstalled?.Id &&
                        storeAndSign.CurrentSchedule.Id != 0 ||
                        storeAndSign?.CurrentSchedule.LastUpdated > storeAndSign?.LastInstalled?.LastUpdated)
                    {
                        _logger.WriteLog($"Starting on store {storeAndSign.Name} ");
                        _createFilesToSend.Init(storeAndSign, _logger, _sendCommunicator, _sendToSignManager);

                        if (SendTheScheduleToSign())
                        {
                            _storeScheduleLogManager.SetValues(storeAndSign);
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

        private bool SendTheScheduleToSign()
        {
            return _createFilesToSend.UploadFileToSign();
        }
    }
}
