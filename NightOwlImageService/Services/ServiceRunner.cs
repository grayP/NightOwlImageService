﻿using System;
using System.Net.Mime;
using System.Timers;
using Autofac;
using ConfigInjector;
using ImageProcessor.Services;
using Logger;
using Logger.Logger;
using nightowlsign.data;
using nightowlsign.data.Models.Stores;
using nightowlsign.data.Models.StoreScheduleLog;
using NightOwlImageService.Configuration;

namespace NightOwlImageService.Services
{
    public class ServiceRunner
    {
        private readonly Timer _timer;
        private readonly MLogger _logger;
        // private RunnerCycleTime runCycleTime;
        private System.Reflection.Assembly _assembly;
        private readonly ImageManager _imageManager;

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
            _imageManager = new ImageManager(_logger);
            _timer.Interval = 300000;
            _timer.Elapsed += (sender, eventArgs) => DoTheWork();
        }

        public void DoTheWork()
        {
            _logger.WriteLog($"Starting Run: {DateTime.Now}");
            var svm = new StoreViewModel { EventCommand = "List" };
            svm.HandleRequest();
            foreach (var storeAndSign in svm.StoresAndSigns)
            {
                if (storeAndSign?.CurrentSchedule.Id != storeAndSign?.LastInstalled?.Id && storeAndSign.CurrentSchedule?.Id != 0 || storeAndSign?.CurrentSchedule.LastUpdated > storeAndSign?.LastInstalled?.LastUpdated)
                {
                    Console.WriteLine($"Starting on store {storeAndSign.Name} ");
                    _logger.WriteLog($"Starting on store {storeAndSign.Name} ");

                    var successCode = SendTheScheduleToSign(storeAndSign);
                    UpdateTheDataBase(storeAndSign, successCode);

                    this._logger.WriteLog(
                        $"ServiceRunner - doTheWork - Uploaded images for {storeAndSign.Name} store, schedule: {storeAndSign.CurrentSchedule.Name}, SuccessCode={successCode}");
                }
            }
            CheckIfTimeToClose();
        }

        private void UpdateTheDataBase(StoreAndSign storeAndSign, int successCode)
        {
            var sm = new StoreManager();
            sm.Update(storeAndSign, successCode);
            if (successCode == 0)
            {
                var sslm = new StoreScheduleLogManager(storeAndSign);
                _logger.WriteLog(sslm.Insert() ? $"Updated {storeAndSign.id}" : $"{sslm.ErrorMessage} ");
            }
        }

        private int SendTheScheduleToSign(StoreAndSign storeAndSign)
        {
            return _imageManager.FileUploadResultCode(storeAndSign);
        }
        private static void CheckIfTimeToClose()
        {
            var timeNow = DateTime.Now.Hour;
            if (timeNow >= 23)
            {
                Environment.Exit(0);
            }
        }
    }
}
