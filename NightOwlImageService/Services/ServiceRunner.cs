using ImageProcessor.Services;
using Logger.Service;
using nightowlsign.data;
using nightowlsign.data.Models.Stores;
using nightowlsign.data.Models.StoreScheduleLog;
using System;
using ScreenBrightness;
using nightowlsign.data.Models.ScreenBrightness;


namespace NightOwlImageService.Services
{
    public class ServiceRunner : IServiceRunner
    {
        private readonly System.Timers.Timer _timer;
        private readonly IGeneralLogger _logger;
        private readonly Inightowlsign_Entities _context;
        private StoreScheduleLogManager _storeScheduleLogManager;
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


        public ServiceRunner(Inightowlsign_Entities context, IStoreScheduleLogManager storeScheduleLogManager)//, IScreenImageManager screenImageManager, , IGeneralLogger logger, IBrightness brightness)
        {
            _context = context;
            _logger = new GeneralLogger(context);
            //_brightness = brightness;
            _timer = new System.Timers.Timer
            {
                AutoReset = true,
                Interval = 300000
            };
            _timer.Elapsed += (sender, eventArgs) => DoTheWork();

        }

        public void DoTheWork()
        {
            var context = new nightowlsign_Entities();
            var storeManager = new StoreManager(context);
            var storeViewModel = new StoreViewModel(storeManager, context);
            _logger.WriteLog($"Starting Run: {DateTime.Now}", "StartUp");

            UpdateSignImages(storeManager, storeViewModel);

            //SetBrightnessLevels(storeViewModel);
            CheckIfTimeToClose();
        }

        private void UpdateSignImages(StoreManager storeManager, StoreViewModel storeViewModel)
        {
            storeViewModel.HandleRequest();
            foreach (var storeAndSign in storeViewModel.StoresAndSigns)
            {
                if (storeAndSign.SignNeedsToBeUpdated())
                {
                    if (storeAndSign.CheckForChangeInSchedule())
                    {
                        storeManager.CleanOutOldSchedule(storeAndSign);
                    }

                    var successCode = SendTheScheduleToSign(storeAndSign);
                    UpdateTheDataBase(storeAndSign, successCode, storeManager);
                    _logger.WriteLog($"Uploaded images for {storeAndSign.Name} store, schedule: {storeAndSign.CurrentSchedule.Name}, SuccessCode={successCode}", "Result");
                }
            }
        }

        private void SetBrightnessLevels(StoreViewModel storeViewModel)
        {
            storeViewModel.HandleRequest();

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

        public void UpdateTheDataBase(StoreAndSign storeAndSign, int successCode, StoreManager storeManager)
        {
            storeManager.Update(storeAndSign, successCode);
            if (successCode == 0)
            {
                _storeScheduleLogManager = new StoreScheduleLogManager();
                _storeScheduleLogManager.Init(storeAndSign);
                if (_storeScheduleLogManager.Insert())
                {
                    _logger.WriteLog($"Updated Log for {storeAndSign.Name}", "Result");
                }
                else
                {
                    _logger.WriteLog($"Store schedule Log not updated for  {storeAndSign.Name} {_storeScheduleLogManager.ErrorMessage}", "Result");
                }
            }
            else
            {
                _logger.WriteLog($"Success code was not 0 for {storeAndSign.Name}", "Result");
            }
        }

        public int SendTheScheduleToSign(StoreAndSign storeAndSign)
        {
            _logger.WriteLog($"Starting on store {storeAndSign.Name} - {storeAndSign.id} ", "Store");
            ScreenImageManager screenImageManager = new ScreenImageManager(_context);
            return screenImageManager.FileUploadResultCode(storeAndSign);
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
