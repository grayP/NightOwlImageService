﻿using System;
using System.Collections.Generic;
using System.IO;
using ImageProcessor.CP5200;
using ImageProcessor.Enums;
using nightowlsign.data;
using nightowlsign.data.Models;
using nightowlsign.data.Models.Signs;
using System.Web;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using Logger;
using nightowlsign.Services;

namespace ImageProcessor.Services

{
    public class CreateFilesToSend
    {
        private readonly SendToSignManager _sendToSignManager = new SendToSignManager();
        private PlayBillFiles _cp5200;
        private const string ImageDirectory = "c:/playBillFiles/Images/";
        private const string ProgramFileDirectory = "c:/playBillFiles/";
        private const string ImageExtension = ".jpg";
        private const string ProgramFileExtension = ".lpb";
        private const string PlaybillFileExtension = ".lpp";

        public string PlaybillFileName { get; set; }
        public bool Successfull { get; set; }

        private readonly Sign _signSizeForSchedule;
        private List<ImageSelect> _imagesToSend;
        private string _scheduleName;
        private StoreAndSign _storeAndSign;
        private mLogger _logger;

        public CreateFilesToSend(StoreAndSign storeAndSign, mLogger logger)
        {
            _storeAndSign = storeAndSign;
            _signSizeForSchedule = storeAndSign.Sign;
            _logger = logger;
        }

        public bool Run()
        {
            try
            {
                _imagesToSend = GetImages(_storeAndSign.CurrentSchedule.Id);
                DeleteOldFiles(ImageDirectory, AddStar(ImageExtension));
                DeleteOldFiles(ProgramFileDirectory, AddStar(ProgramFileExtension));
                DeleteOldFiles(ProgramFileDirectory, AddStar(PlaybillFileExtension));
                WriteImagesToDisk(_imagesToSend);
                GeneratetheProgramFiles(_storeAndSign.CurrentSchedule.Name);
                GeneratethePlayBillFile(_storeAndSign.CurrentSchedule.Name);
                SendCommunicator senderCommunicator = new SendCommunicator(_storeAndSign, ProgramFileDirectory, _logger);
                return senderCommunicator.Run();
           }
            catch (Exception ex)
            {
                _logger.WriteLog($"CreateFilesToSend - Run - {ex.Message}");
                return false;
            }
        }

        private List<ImageSelect> GetImages(int scheduleId)
        {
            return _sendToSignManager.GetImagesForThisSchedule(scheduleId);
        }

        private string AddStar(string fileExtension)
        {
            return string.Concat("*", fileExtension);
        }

        public void DeleteOldFiles(string directoryName, string extension)
        {
            foreach (
                string fileName in Directory.GetFiles(directoryName, extension))
            {
                System.IO.File.Delete(fileName);
            }
        }

        private void WriteImagesToDisk(List<ImageSelect> images)
        {
            foreach (var image in images)
            {
                SaveImageToFile(string.Format("{0:0000}0000", RandomNumber.GenerateRandomNo()), image);
                image.Dispose();
            }
        }

        public void GeneratetheProgramFiles(string scheduleName)
        {
            var PlayItemNo = -1;
            var PeriodToShowImage = 0xA; //Seconds
            byte colourMode = 0x77;
            // ProgramFiles = new List<string>();

            ushort screenWidth = (ushort)(_signSizeForSchedule.Width ?? 100);
            ushort screenHeight = (ushort)(_signSizeForSchedule.Height ?? 100);
            _cp5200 = new PlayBillFiles(screenWidth, screenHeight, PeriodToShowImage, colourMode,_logger);

            var counter = 1;
            foreach (
                string fileName in Directory.GetFiles(ImageDirectory, AddStar(ImageExtension)))
            {
                var programPointer = _cp5200.Program_Create();
                if (programPointer.ToInt32() > 0)
                {
                    var windowNo = _cp5200.AddPlayWindow(programPointer);
                    if (windowNo >= 0)
                    {

                        PlayItemNo = _cp5200.Program_Add_Image(programPointer, windowNo,
                            Marshal.StringToHGlobalAnsi(fileName), (int)RenderMode.Stretch_to_fit_the_window,
                           0, 100, PeriodToShowImage, 0);

                        var programFileName = GenerateProgramFileName(string.Format("{0:0000}0000", counter));
                        DeleteOldProgramFile(programFileName);
                        if (
                            _cp5200.Program_SaveFile(programPointer, programFileName) > 1)
                        {
                            _cp5200.DestroyProgram(programPointer);
                        }
                    }
                }
                counter += 1;
            }
        }

        private void DeleteOldProgramFile(string fileAndPath)
        {
            System.IO.File.Delete(fileAndPath);
        }

        public void GeneratethePlayBillFile(string scheduleName)
        {
            var playBillPointer = _cp5200.playBill_Create();

            if (playBillPointer.ToInt32() > 0)
            {
                if (playBillPointer.ToInt32() > 0)
                    _cp5200.Playbill_SetProperty(playBillPointer, 0, 1);
                foreach (
                    string programFileName in
                    Directory.GetFiles(ProgramFileDirectory, AddStar(ProgramFileExtension)))
                {
                    _cp5200.Playbill_AddFile(playBillPointer, programFileName);
                }
                _cp5200.Playbill_SaveToFile(playBillPointer, GeneratePlayBillFileName(scheduleName));
                _cp5200.DestroyProgram(playBillPointer);
            }

        }

        private void SaveImageToFile(string sCounter, ImageSelect image)
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
                Console.WriteLine(ex.InnerException?.ToString());
            }
        }

        private string GeneratePlayBillFileName(string scheduleName)
        {
            var newName = StripCharacters.Strip(scheduleName);
            return PlaybillFileName = string.Concat(ProgramFileDirectory, newName.Substring(0, Math.Min(8, newName.Length)), PlaybillFileExtension);
        }

        private string GenerateProgramFileName(string sCounter)
        {
            return string.Concat(ProgramFileDirectory, sCounter, ProgramFileExtension);
        }
    }
}


