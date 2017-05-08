﻿using ImageProcessor.Enums;
using Logger.Logger;
using nightowlsign.data;
using nightowlsign.data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using nightowlsign.data.Models.SendToSign;

namespace ImageProcessor.Services

{
    public class ScreenImageManager : IScreenImageManager
    {
        private readonly ISendToSignManager _sendToSignManager;
        private PlayBillFiles _cp5200;

        private string ImageDirectory = "c:/playBillFiles/Images/";
        private readonly string _programFileDirectory = string.Concat(System.IO.Directory.GetCurrentDirectory(), "\\");
        private const string ImageExtension = ".jpg";
        private const string ProgramFileExtension = ".lpb";
        private const string PlaybillFileExtension = ".lpp";

        public string PlaybillFileName { get; set; }
        public bool Successfull { get; set; }

        private List<ImageSelect> _imagesToSend;
        private StoreAndSign _storeAndSign;
        private Sign _signSizeForSchedule;
        private readonly IMLogger _logger;
        private readonly ISendCommunicator _sendCommunicator;

        public StoreAndSign StoreAndSign { get; set; }


        public ScreenImageManager(IMLogger logger, ISendCommunicator sendCommunicator, ISendToSignManager sendToSignManager)
        {
            _logger = logger;
            _sendCommunicator = sendCommunicator;
            _sendToSignManager = sendToSignManager;
        }

        public int FileUploadResultCode(StoreAndSign storeAndSign)
        {
            try
            {
                _imagesToSend = GetImages(storeAndSign.CurrentSchedule.Id);
                DeleteOldFiles(ImageDirectory, AddStar(ImageExtension));
                DeleteOldFiles(_programFileDirectory, AddStar(ProgramFileExtension));
                WriteImagesToDisk(_imagesToSend);
                GeneratetheProgramFiles(storeAndSign.CurrentSchedule.Name, storeAndSign.ProgramFile, storeAndSign.Sign);
                //using (var sender = new SendCommunicator(storeAndSign, _programFileDirectory, _logger))
                //{
                _sendCommunicator.Init(storeAndSign, _programFileDirectory);
                    if (_sendCommunicator.FilesUploadedOk())
                    {
                        _sendCommunicator.RestartSign();
                    }
                    _sendCommunicator.Disconnect();
                    return _sendCommunicator.UpLoadSuccess;
                //}
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"ImageManager - FileUploadResultCode - {ex.Message}");
                return 99;
            }
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

        public void GeneratetheProgramFiles(string scheduleName, string programFile, Sign sign)
        {
            var PlayItemNo = -1;
            var PeriodToShowImage = 0xA; //Seconds
            byte colourMode = 0x77;
            // ProgramFiles = new List<string>();

            ushort screenWidth = (ushort)(sign.Width ?? 100);
            ushort screenHeight = (ushort)(sign.Height ?? 100);
            _cp5200 = new PlayBillFiles(screenWidth, screenHeight, PeriodToShowImage, colourMode, _logger);

            var programPointer = _cp5200.Program_Create();
            if (programPointer.ToInt32() > 0)
            {
                var windowNo = _cp5200.AddPlayWindow(programPointer);
                if (windowNo >= 0)
                {
                    foreach (
                        string fileName in Directory.GetFiles(ImageDirectory, AddStar(ImageExtension)))
                    {

                        PlayItemNo = _cp5200.Program_Add_Image(programPointer, windowNo,
                            Marshal.StringToHGlobalAnsi(fileName), (int)RenderMode.Stretch_to_fit_the_window,
                           0, 100, PeriodToShowImage, 0);
                    }
                }
                var programFileName = GenerateProgramFileName(programFile);
                DeleteOldProgramFile(programFileName);
                if (
                    _cp5200.Program_SaveFile(programPointer, programFileName) > 1)
                {
                    _cp5200.DestroyProgram(programPointer);
                }
            }
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
            string tempFileName = string.Concat(ImageDirectory, sCounter, ImageExtension);
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(image.ImageUrl, tempFileName);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"Error save image to disk - {ex.InnerException?.ToString()}");
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


