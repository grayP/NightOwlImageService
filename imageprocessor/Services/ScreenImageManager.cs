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

namespace ImageProcessor.Services

{
    public class ScreenImageManager : IScreenImageManager
    {
        private readonly ISendToSignManager _sendToSignManager;
        private PlayBillFiles _cp5200;

        private string _imageDirectory = "c:/playBillFiles/Images/";
        private string _programFileDirectory = "c:/programFiles/"; // string.Concat(System.IO.Directory.GetCurrentDirectory(), "\\");
        private const string ImageExtension = ".jpg";
        private const string ProgramFileExtension = ".lpb";
        private const string PlaybillFileExtension = ".lpp";

        public string PlaybillFileName { get; set; }
        public bool Successfull { get; set; }

        private List<ImageSelect> _imagesToSend;
        private readonly IGeneralLogger _logger;
        private readonly ISendCommunicator _sendCommunicator;

        public StoreAndSign StoreAndSign { get; set; }


        public ScreenImageManager(IGeneralLogger logger, ISendCommunicator sendCommunicator, ISendToSignManager sendToSignManager)
        {
            _logger = logger;
            _sendCommunicator = sendCommunicator;
            _sendToSignManager = sendToSignManager;
        }

        public int FileUploadResultCode(StoreAndSign storeAndSign)
        {
            try
            {
                UpdateTheStorageDirectory(storeAndSign);
                UpdateTheImageDirectory();
                _imagesToSend = GetImages(storeAndSign.CurrentSchedule.Id);
                DeleteOldFiles(_imageDirectory, AddStar(ImageExtension));
                DeleteOldFiles(_programFileDirectory, AddStar(ProgramFileExtension));
                WriteImagesToDisk(_imagesToSend);
                GeneratetheProgramFiles(storeAndSign);
                _sendCommunicator.Init(storeAndSign, _programFileDirectory);
                if (_sendCommunicator.FilesUploadedOk())
                {
                    _sendCommunicator.RestartSign();
                }
                _sendCommunicator.Disconnect();
                return _sendCommunicator.UpLoadSuccess;
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"ImageManager - FileUploadResultCode - {ex.Message}", "Error");
                return 99;
            }
        }

        private void UpdateTheStorageDirectory(StoreAndSign storeAndSign)
        {
            _programFileDirectory = $"c:/programFiles/{storeAndSign.id}/";
            Directory.CreateDirectory(_programFileDirectory);
        }

        private void UpdateTheImageDirectory()
        {
            _imageDirectory = $"{_programFileDirectory}images/";
            Directory.CreateDirectory(_imageDirectory);
        }

        public List<ImageSelect> GetImages(int scheduleId)
        {
            return _sendToSignManager.GetImagesForThisSchedule(scheduleId);
        }

        private static string AddStar(string fileExtension)
        {
            return string.Concat("*", fileExtension);
        }

        public void DeleteOldFiles(string directoryName, string extension)
        {
            foreach (
                var fileName in Directory.GetFiles(directoryName, extension))
            {
                System.IO.File.Delete(fileName);
            }
        }

        public void WriteImagesToDisk(IEnumerable<ImageSelect> images)
        {
            var counter = 1;
            foreach (var image in images)
            {
                SaveImageToFile(string.Format("{0:0000}0000", counter), image);
                image.Dispose();
                counter++;
            }
        }

        public void GeneratetheProgramFiles(StoreAndSign storeAndSign)
        {
            var programFile = storeAndSign.ProgramFile;
            int imagesInFile = storeAndSign.NumImages ?? 12;
            const int periodToShowImage = 0xA; //Seconds
            const byte colourMode = 0x77;
            var screenWidth = (ushort)(storeAndSign.Sign.Width ?? 100);
            var screenHeight = (ushort)(storeAndSign.Sign.Height ?? 100);
            var images = Directory.GetFiles(_imageDirectory, AddStar(ImageExtension));
            storeAndSign.NumImagesUploaded = images.Length;
            int numProgramFiles =  (int)Math.Ceiling((double)images.Length/imagesInFile);
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
                            for (int j = i * imagesInFile; j <= ((i+1) * imagesInFile - 1); j++)
                            {
                                if (j < images.Length)
                                {
                                    cp5200.Program_Add_Image(programPointer, windowNo, Marshal.StringToHGlobalAnsi(images[j]), (int)RenderMode.Stretch_to_fit_the_window, 0, 100, periodToShowImage, 1);
                                }
                            }
                        }
                        var result = cp5200.Program_SaveFile(programPointer, GenerateProgramFileName(Increment(programFile, i)));
                        cp5200.DestroyProgram(programPointer);
                    }
                }
            }
        }

        private static string Increment(string fileName, int i)
        {
            int number;
            var success = Int32.TryParse(fileName, out number);
            if (!success) return fileName;
            var answer = $"000000{number + i}";
            return answer.Substring(answer.Length - 8);
        }


        public void DeleteOldProgramFile(string fileAndPath)
        {
            System.IO.File.Delete(fileAndPath);
        }

        public void GeneratethePlayBillFile(string scheduleName)
        {
            var playBillPointer = _cp5200.playBill_Create();

            if (playBillPointer.ToInt32() > 0)
            {
                _cp5200.Playbill_SetProperty(playBillPointer, 0, 1);
                foreach (string programFileName in
                    Directory.GetFiles(_programFileDirectory, AddStar(ProgramFileExtension)))
                {
                    _cp5200.Playbill_AddFile(playBillPointer, programFileName);
                }
                _cp5200.Playbill_SaveToFile(playBillPointer, GeneratePlayBillFileName(scheduleName));
                _cp5200.DestroyProgram(playBillPointer);
            }

        }

        public void SaveImageToFile(string sCounter, ImageSelect image)
        {
            string tempFileName = string.Concat(_imageDirectory, sCounter, ImageExtension);
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(image.ImageUrl, tempFileName);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"Error save image to disk - {ex.InnerException?.ToString()}","Error");
            }
        }

        public string GeneratePlayBillFileName(string scheduleName)
        {
            var newName = "playbill"; //StripCharacters.Strip(scheduleName);
            return PlaybillFileName = string.Concat(_programFileDirectory, newName.Substring(0, Math.Min(8, newName.Length)), PlaybillFileExtension);
        }

        public string GenerateProgramFileName(string programFile)
        {
            if (string.IsNullOrEmpty(programFile))
            {
                programFile = "00010000";
            }
            return string.Concat(_programFileDirectory, programFile, ".lpb");
        }
    }
}



