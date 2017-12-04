using ImageProcessor.Services;
using Logger.Service;
using nightowlsign.data;
using nightowlsign.data.Models.Stores;
using nightowlsign.data.Models.StoreScheduleLog;
using System;
using ScreenBrightness;
using nightowlsign.data.Models.ScreenBrightness;
using System.Linq;

namespace NightOwlImageService.Services
{
    public class ServiceRunner : IServiceRunner
    {
        private readonly System.Timers.Timer _timer;
        private readonly IGeneralLogger _logger;
        private readonly Inightowlsign_Entities _context;
        private StoreScheduleLogManager _storeScheduleLogManager;
        private readonly IStoreManager _storeManager;
        private readonly IStoreViewModel _storeViewModel;

        //private readonly IScreenImageManager _screenImageManager;
        //private readonly IBrightness _brightness;
        // private RunnerCycleTime runCycleTime;

        public void Start()
        {
            DoTheWork();
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public ServiceRunner(Inightowlsign_Entities context, IStoreManager storeManager, IStoreViewModel storeViewModel)//, IStoreManager storeManager, IGeneralLogger logger)//, IScreenImageManager screenImageManager, , IBrightness brightness)
        {
            _context = context;
            _storeManager = storeManager;
            _storeViewModel = storeViewModel; // new StoreViewModel(_storeManager, _context);

            _logger = new GeneralLogger(context);
            //_brightness = brightness;
            _timer = new System.Timers.Timer
            {
                AutoReset = true,
                Interval = 600000
            };
            _timer.Elapsed += (sender, eventArgs) => DoTheWork();

        }

        public void DoTheWork()
        {
            _logger.WriteLog($"Starting Run: {DateTime.Now}", "StartUp");
            _storeViewModel.HandleRequest();
            UpdateSignImages(_storeManager, _storeViewModel);
            //SetBrightnessLevels(storeViewModel);
            CheckIfTimeToClose();
        }

        private void UpdateSignImages(IStoreManager _storeManager, IStoreViewModel _storeViewModel)
        {
            foreach (var storeAndSign in _storeViewModel.StoresAndSigns.Where(x => x.SignNeedsToBeUpdated() == true && x.Active == true))// && x.SignNeedsToBeUpdated()))
            {

//                if (storeAndSign.SignNeedsToBeUpdated())
  //              {
                    // CleanOutOldSchedule(storeAndSign, _storeManager);
                    SendTheScheduleToSign(storeAndSign);
                    UpdateTheDataBase(storeAndSign, _storeManager);
                    _logger.WriteLog($"Uploaded images for {storeAndSign.Name} store, schedule: {storeAndSign.CurrentSchedule.Name}, SuccessCode={storeAndSign.SuccessCode}", "Result");
    //            }
            }
        }

        private void CleanOutOldSchedule(StoreAndSign storeAndSign, IStoreManager storeManager)
        {
            storeManager.CleanOutOldSchedule(storeAndSign);
        }

        private void SetBrightnessLevels(IStoreViewModel storeViewModel)
        {
            foreach (var storeAndSign in storeViewModel.StoresAndSigns)
            {
                if (storeAndSign.Name == "West End")
                {
                    ScreenBrightnessManager sbm = new ScreenBrightnessManager();
                    if (storeAndSign.BrightnessNeedsToBeSet(sbm, storeAndSign.id))
                    {
                        ScreenBrightnessSetter brightness = new ScreenBrightnessSetter(storeAndSign.IpAddress, storeAndSign.Port, sbm.currentBrightness);
                        brightness.SetBrightness();
                    }
                }
            }
        }

        public void UpdateTheDataBase(StoreAndSign storeAndSign, IStoreManager storeManager)
        {
            DateTime dateUpdated = DateTime.Now;
            storeManager.Update(storeAndSign, dateUpdated);
            if (storeAndSign.SuccessCode == 0)
            {
                _storeScheduleLogManager = new StoreScheduleLogManager();
                _storeScheduleLogManager.Init(storeAndSign);
                var result = _storeScheduleLogManager.Insert(dateUpdated);
                _logger.WriteLog($"Updated Log for {storeAndSign.Name}-{result.ToString()}", "Result");
            }
            else
            {
                _logger.WriteLog($"Success code was not 0 for {storeAndSign.Name}", "Result");
            }
        }

        public void SendTheScheduleToSign(StoreAndSign storeAndSign)
        {
            _logger.WriteLog($"Starting on store {storeAndSign.Name} - {storeAndSign.id} ", "Store");
            ScreenImageManager screenImageManager = new ScreenImageManager(_context);
            screenImageManager.UpLoadFileToSign(storeAndSign);
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
