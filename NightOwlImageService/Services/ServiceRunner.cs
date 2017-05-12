using System;
using System.Net.Mime;
using System.Timers;
using Autofac;
using ConfigInjector;
using ImageProcessor.Services;
using Logger;
using Logger.Service;
using nightowlsign.data;
using nightowlsign.data.Models.Image;
using nightowlsign.data.Models.Stores;
using nightowlsign.data.Models.StoreScheduleLog;
using NightOwlImageService.Configuration;

namespace NightOwlImageService.Services
{
    public class ServiceRunner : IServiceRunner
    {
        private readonly Timer _timer;
        private readonly IGeneralLogger _logger;
        private readonly IStoreManager _storeManager;
        private readonly IStoreViewModel _storeViewModel;
        private readonly Inightowlsign_Entities _context;
        private readonly IStoreScheduleLogManager _storeScheduleLogManager;
        private readonly IScreenImageManager _screenImageManager;
        // private RunnerCycleTime runCycleTime;

        public void Start()
        {
            DoTheWork(_storeViewModel);
            _timer.Start();
        }


        public void Stop()
        {
            _timer.Stop();
        }


        public ServiceRunner(IScreenImageManager screenImageManager, IStoreManager storeManager, IStoreViewModel storeViewModel, Inightowlsign_Entities context, IStoreScheduleLogManager storeScheduleLogManager, IGeneralLogger logger)
        {
            _screenImageManager = screenImageManager;
            _storeManager = storeManager;
            _storeViewModel = storeViewModel;
            _context = context;
            _storeScheduleLogManager = storeScheduleLogManager;

            _logger = logger; 

            _logger.Init(System.Reflection.Assembly.GetExecutingAssembly().FullName);
            _timer = new Timer
            {
                AutoReset = true,
                Interval = 300000
            };
            _timer.Elapsed += (sender, eventArgs) => DoTheWork(_storeViewModel);
        }

        public void DoTheWork(IStoreViewModel storeViewModel)
        {
            _logger.WriteLog($"Starting Run: {DateTime.Now}","StartUp");
            storeViewModel.EventCommand = "List";
            storeViewModel.HandleRequest();
            foreach (var storeAndSign in storeViewModel.StoresAndSigns)
            {
                if (storeAndSign.SignNeedsToBeUpdated())
                {

                    var successCode = SendTheScheduleToSign(storeAndSign);
                    UpdateTheDataBase(storeAndSign, successCode);

                    this._logger.WriteLog(
                        $"Uploaded images for {storeAndSign.Name} store, schedule: {storeAndSign.CurrentSchedule.Name}, SuccessCode={successCode}" ,"Result");
                }
            }
            CheckIfTimeToClose();
        }

        public void UpdateTheDataBase(StoreAndSign storeAndSign, int successCode)
        {
            _storeManager.Update(storeAndSign, successCode);              
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
