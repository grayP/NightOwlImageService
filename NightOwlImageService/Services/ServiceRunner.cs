﻿using ImageProcessor.Services;
using Logger.Service;
using nightowlsign.data;
using nightowlsign.data.Models.Stores;
using nightowlsign.data.Models.StoreScheduleLog;
using System;
using System.Timers;
using nightowlsign.data.Models.UpLoadLog;

namespace NightOwlImageService.Services
{
    public class ServiceRunner : IServiceRunner
    {
        private readonly Timer _timer;
        private readonly IGeneralLogger _logger;
        private readonly Inightowlsign_Entities _context;
        private readonly IStoreScheduleLogManager _storeScheduleLogManager;
        private readonly IScreenImageManager _screenImageManager;
        // private RunnerCycleTime runCycleTime;

        public void Start()
        {
            DoTheWork(_context);
            _timer.Start();
        }


        public void Stop()
        {
            _timer.Stop();
        }


        public ServiceRunner(IScreenImageManager screenImageManager, Inightowlsign_Entities context, IStoreScheduleLogManager storeScheduleLogManager, IGeneralLogger logger)
        {
            _screenImageManager = screenImageManager;
            _context = context;
            _storeScheduleLogManager = storeScheduleLogManager;

            _logger = logger; 

            _logger.Init(System.Reflection.Assembly.GetExecutingAssembly().FullName);
            _timer = new Timer
            {
                AutoReset = true,
                Interval = 300000
            };
            _timer.Elapsed += (sender, eventArgs) => DoTheWork(_context);
        }

        public void DoTheWork(Inightowlsign_Entities context)
        {
            var storeManager= new StoreManager(context);
            var storeViewModel = new StoreViewModel(storeManager);
            _logger.WriteLog($"Starting Run: {DateTime.Now}","StartUp");
            storeViewModel.EventCommand = "List";
            storeViewModel.HandleRequest();
            foreach (var storeAndSign in storeViewModel.StoresAndSigns)
            {
                //this._logger.WriteLog($"Debug: {storeAndSign.Name}, Curr: {storeAndSign.CurrentSchedule?.LastUpdated}, Last:{storeAndSign.LastInstalled?.LastUpdated}, Current: {storeAndSign.CurrentSchedule?.Id}, Last: {storeAndSign.LastInstalled?.Id}   ");
                if (storeAndSign.SignNeedsToBeUpdated())
                {
                    if (storeAndSign.CurrentSchedule.Id != storeAndSign.LastInstalled.Id)
                    {
                        CleanOutTheUpLoadLog(storeAndSign, context);
                    }
                    var successCode = SendTheScheduleToSign(storeAndSign);
                    UpdateTheDataBase(storeAndSign, successCode, storeManager);
                    this._logger.WriteLog($"Uploaded images for {storeAndSign.Name} store, schedule: {storeAndSign.CurrentSchedule.Name}, SuccessCode={successCode}" ,"Result");
                }
            }
            CheckIfTimeToClose();
        }

        private void CleanOutTheUpLoadLog(StoreAndSign storeAndSign, Inightowlsign_Entities context)
        {
            var uploadLoggingManager = new UpLoadLoggingManager(context);
            uploadLoggingManager.Delete(storeAndSign.id, storeAndSign.CurrentSchedule.Id);
            uploadLoggingManager.Delete(storeAndSign.id, storeAndSign.LastInstalled.Id);
        }

        public void UpdateTheDataBase(StoreAndSign storeAndSign, int successCode, IStoreManager storeManager)
        {
            storeManager.Update(storeAndSign, successCode);              
            if (successCode == 0)
            {
                _storeScheduleLogManager.Init(storeAndSign);
                _logger.WriteLog(_storeScheduleLogManager.Insert() ? $"Updated Log for {storeAndSign.Name}" : $"{_storeScheduleLogManager.ErrorMessage} ","Result");
            }
        }

        public int SendTheScheduleToSign(StoreAndSign storeAndSign)
        {
            Console.WriteLine($"Starting on store {storeAndSign.Name} ");
            _logger.WriteLog($"Starting on store {storeAndSign.Name} ", "Store");
            return _screenImageManager.FileUploadResultCode(storeAndSign);
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
