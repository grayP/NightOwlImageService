using ImageProcessor.Enums;
using nightowlsign.data;
using nightowlsign.data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Web.UI.WebControls;
using Logger.Service;
using nightowlsign.data.Models.SendToSign;
using nightowlsign.data.Models.UpLoadLog;
using System.Linq;

namespace ImageProcessor.Services

{
    public class ScreenImageManager : IScreenImageManager
    {
        private readonly SendToSignManager sendToSignManager;
        private readonly SendCommunicator sendCommunicator;
        private PlayBillFiles _cp5200;

        //private string _programFileDirectory = "D:\\local\\Temp\\"; // string.Concat(System.IO.Directory.GetCurrentDirectory(), "\\");
        private string _programFileDirectory = "c:\\programFiles\\"; // string.Concat(System.IO.Directory.GetCurrentDirectory(), "\\");
        private string _storeProgramDirectory;
        private string _imageDirectory;
        private const string ImageExtension = ".jpg";
        private const string ProgramFileExtension = ".lpb";
        private const string PlaybillFileExtension = ".lpp";

        public string PlaybillFileName { get; set; }
        public bool Successfull { get; set; }

        private List<ImageSelect> _imagesToSend;
        private readonly GeneralLogger _logger;

        public StoreAndSign StoreAndSign { get; set; }


        public ScreenImageManager(Inightowlsign_Entities context) //IGeneralLogger logger, ISendCommunicator sendCommunicator, ISendToSignManager sendToSignManager)
        {
            _logger = new GeneralLogger(context);
            UpLoadLoggingManager upLoadLoggingManager = new UpLoadLoggingManager(context);
            UpLoadLogger uploadLogger = new UpLoadLogger(upLoadLoggingManager);
            sendCommunicator = new SendCommunicator(_logger, uploadLogger);
            sendToSignManager = new SendToSignManager();
        }

        public void UpLoadFileToSign(StoreAndSign storeAndSign)
        {
            try
            {
                UpdateTheStorageDirectory(storeAndSign);
                _imagesToSend = GetImages(storeAndSign.CurrentSchedule.Id);
                WriteImagesToDisk(_imageDirectory, _imagesToSend);
                CleanOutTheProgramFiles(_storeProgramDirectory);
                GeneratetheProgramFiles(_storeProgramDirectory, storeAndSign);
                //GeneratethePlayBillFile(storeAndSign);
                SendImagesToSign(storeAndSign, _storeProgramDirectory);

            }
            catch (Exception ex)
            {
                _logger.WriteLog($"ImageManager -  {ex.Message}{ex.InnerException}", "Error");
                storeAndSign.SuccessCode = -99;
            }
        }

        private void CleanOutTheProgramFiles(string storeProgramDirectory)
        {
            try
            {
                Directory.GetFiles(storeProgramDirectory, $"*{ProgramFileExtension}").ToList().ForEach(f => { File.Delete(f); });
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"ScreenImageManager - CleanOutTheProgramFiles-{ex.Message}{ex.InnerException}", "Error");
            }
        }

        public void SendImagesToSign(StoreAndSign storeAndSign, string programFileDirectory)
        {
            if (sendCommunicator.FilesUploadedOk(storeAndSign, programFileDirectory))
            {
                sendCommunicator.RestartSign();
            }
            sendCommunicator.Disconnect();
            storeAndSign.SuccessCode = sendCommunicator.UpLoadSuccess;
        }

        private void UpdateTheStorageDirectory(StoreAndSign storeAndSign)
        {
            _storeProgramDirectory = $"{_programFileDirectory}{storeAndSign.id}/";
            Directory.CreateDirectory(_storeProgramDirectory);
            _imageDirectory = $"{_storeProgramDirectory}images/";
            Directory.CreateDirectory(_imageDirectory);
        }

        public List<ImageSelect> GetImages(int scheduleId)
        {
            SendToSignManager sendToSignManager = new SendToSignManager();
            return sendToSignManager.GetImagesForThisSchedule(scheduleId);
        }

        private static string AddStar(string fileExtension)
        {
            return string.Concat("*", fileExtension);
        }

        public void DeleteOldFiles(string directoryName, string extension)
        {
        }

        public void WriteImagesToDisk(string imageDirectory, IEnumerable<ImageSelect> images)
        {
            try
            {
                foreach (var fileName in Directory.GetFiles(imageDirectory, "*.jpg"))
                {
                    System.IO.File.Delete(fileName);
                }

                var counter = 1;
                foreach (var image in images)
                {
                    SaveImageToFile(imageDirectory, string.Format("{0:0000}0000", counter), image);
                    image.Dispose();
                    counter++;
                }
            }
            catch (Exception ex)
            {

                _logger.WriteLog($"WriteImagesToDisk - {ex.Message}{ex.InnerException}", "Error");
            }
        }

        public void GeneratetheProgramFiles(string programFileDirectory, StoreAndSign storeAndSign)
        {
            var programFile = storeAndSign.ProgramFile;
            int imagesInFile = storeAndSign.NumImages ?? 12;
            const int periodToShowImage = 0xA; //Seconds
            const byte colourMode = 0x77;
            var screenWidth = (ushort)(storeAndSign.Sign.Width ?? 100);
            var screenHeight = (ushort)(storeAndSign.Sign.Height ?? 100);
            var images = Directory.GetFiles(_imageDirectory, AddStar(ImageExtension));
            storeAndSign.NumImagesUploaded = images.Length;
            int numProgramFiles = (int)Math.Ceiling((double)images.Length / imagesInFile);



            for (int i = 0; i < numProgramFiles; i++)
            {
                using (var cp5200 = new PlayBillFiles(screenWidth, screenHeight, periodToShowImage, colourMode, _logger))
                {
                    var programPointer = cp5200.Program_Create();
                    if (programPointer.ToInt32() > 0)
                    {
                        var windowNo = cp5200.AddPlayWindow(programPointer);
                        if (windowNo >= 0)
                        {
                            for (int j = i * imagesInFile; j <= ((i + 1) * imagesInFile - 1); j++)
                            {
                                if (j < images.Length)
                                {
                                    cp5200.Program_Add_Image(programPointer, windowNo, Marshal.StringToHGlobalAnsi(images[j]), (int)RenderMode.Stretch_to_fit_the_window, 0, 100, periodToShowImage, 1);
                                }
                            }
                        }
                        var result = cp5200.Program_SaveFile(programPointer, GenerateProgramFileName(programFileDirectory, Increment(programFile, i)));
                        cp5200.DestroyProgram(programPointer);
                    }
                }
            }
        }

        private static string Increment(string fileName, int i)
        {
            int number;
            fileName = $"{fileName}00";
            var success = Int32.TryParse(fileName.Substring(6, 2), out number);
            if (!success) return fileName;
            var answer = $"00{number + i}";
            return $"{fileName.Substring(0, 5)}{answer}";
        }


        public void DeleteOldProgramFile(string fileAndPath)
        {
            File.Delete(fileAndPath);
        }

        public void GeneratethePlayBillFile(StoreAndSign storeAndSign)
        {
            using (var playBill = new PlayBillFiles(Convert.ToInt32(storeAndSign.Sign.Width.Value), Convert.ToInt32(storeAndSign.Sign.Height.Value), 0xA, 0x77, _logger))
            {
                var playBillPointer = playBill.playBill_Create();

                if (playBillPointer.ToInt32() > 0)
                {
                    playBill.Playbill_SetProperty(playBillPointer, 0, 1);
                    foreach (string programFileName in
                        Directory.GetFiles(_programFileDirectory, AddStar(ProgramFileExtension)))
                    {
                        playBill.Playbill_AddFile(playBillPointer, programFileName);
                    }
                    playBill.Playbill_SaveToFile(playBillPointer, GeneratePlayBillFileName("playbill"));
                    playBill.DestroyProgram(playBillPointer);
                }
            };
        }

        public void SaveImageToFile(string imageDirectory, string sCounter, ImageSelect image)
        {
            string tempFileName = string.Concat(imageDirectory, sCounter, ImageExtension);
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(image.ImageUrl, tempFileName);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"Error save image to disk - {ex.Message}", "Error");
            }
        }

        public string GeneratePlayBillFileName(string scheduleName)
        {
            var newName = "playbill"; //StripCharacters.Strip(scheduleName);
            return PlaybillFileName = string.Concat(_programFileDirectory, newName.Substring(0, Math.Min(8, newName.Length)), PlaybillFileExtension);
        }

        public string GenerateProgramFileName(string programFileDirectory, string programFile)
        {
            return string.Concat(programFileDirectory, programFile ?? "00010000", ".lpb");
        }
    }
}



