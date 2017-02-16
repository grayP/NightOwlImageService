using System;
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
using nightowlsign.Services;
namespace ImageProcessor.Services

{
    public class CreateFilesToSend
    {
        private PlayBillFiles _cp5200;
        private const string ImageDirectory = "~/playBillFiles/Images/";
        private const string ProgramFileDirectory = "~/playBillFiles/";
        private const string ImageExtension = ".jpg";
        private const string ProgramFileExtension = ".lpb";
        private const string PlaybillFileExtension = ".lpp";

        public List<string> ProgramFiles { get; set; }
        public string PlaybillFileName { get; set; }

        private readonly List<SignDto> _signSizesForSchedule;
        private readonly List<ImageSelect> _imagesToSend;
        private readonly SignManager sm = new SignManager();
        private string _scheduleName;

        public string DebugString { get; set; }

        public CreateFilesToSend(List<SignDto> signsForSchedule, List<ImageSelect> imagesToSend, string schedulename)
        {
            _signSizesForSchedule = signsForSchedule;
            _imagesToSend = imagesToSend;
            _scheduleName = schedulename;
            Run();
        }

        private void Run()
        {
            DeleteOldFiles(ImageDirectory, AddStar(ImageExtension));
            DeleteOldFiles(ProgramFileDirectory, AddStar(ProgramFileExtension));
            DeleteOldFiles(ProgramFileDirectory, AddStar(PlaybillFileExtension));
            WriteImagesToDisk();
            GeneratetheProgramFiles(_scheduleName);
            GeneratethePlayBillFile(_scheduleName);
        }

        private string AddStar(string fileExtension)
        {
            return string.Concat("*",fileExtension);
        }

        public void DeleteOldFiles( string directoryName,string extension)
        {
            foreach (
                string fileName in Directory.GetFiles(HttpContext.Current.Server.MapPath(directoryName), extension))
            {
                System.IO.File.Delete(fileName);
            }
        }

        public void PopulateSignList()
        {
            foreach (var signDto in _signSizesForSchedule)
            {
                sm.Find(signDto.Id);
            }
        }
        public void WriteImagesToDisk()
        {
            foreach (var image in _imagesToSend)
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
            ProgramFiles = new List<string>();
            foreach (var signSize in _signSizesForSchedule)
            {
                ushort screenWidth = (ushort)(signSize.Width);
                ushort screenHeight = (ushort)(signSize.Height);
                _cp5200 = new PlayBillFiles(screenWidth, screenHeight, PeriodToShowImage, colourMode);

                var counter = 1;
                foreach (
                    string fileName in Directory.GetFiles(HttpContext.Current.Server.MapPath(ImageDirectory), AddStar(ImageExtension)))
                {
                    var programPointer = _cp5200.Program_Create();
                    if (programPointer.ToInt32() > 0)
                    {
                        var windowNo = _cp5200.AddPlayWindow(programPointer);
                        if (windowNo >= 0)
                        {
                            //PlayItemNo = cp5200.Program_AddPicture(programPointer, fileName,
                            //    (int)RenderMode.Stretch_to_fit_the_window, 0,
                            //    0, PeriodToShowImage, 0);

                            PlayItemNo = _cp5200.Program_Add_Image(programPointer, windowNo,
                                Marshal.StringToHGlobalAnsi(fileName), (int) RenderMode.Stretch_to_fit_the_window,
                               0, 100, PeriodToShowImage, 0);

        //                    DebugString += string.Format("{0}Play Item Number: {1}, Temp File Name  {2}",
         //                       Environment.NewLine, PlayItemNo, fileName);

                            var programFileName = GenerateProgramFileName(string.Format("{0:0000}0000", counter));
                            DeleteOldProgramFile(programFileName);
                            // ProgramFiles.Add(programFileName);
                            if (
                                _cp5200.Program_SaveFile(programPointer,programFileName) > 1)
                            {
                                _cp5200.DestroyProgram(programPointer);
                            }
                        }
                    }
                    counter += 1;
                }
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
                    Directory.GetFiles(HttpContext.Current.Server.MapPath(ProgramFileDirectory), AddStar(ProgramFileExtension)))
                {
                    _cp5200.Playbill_AddFile(playBillPointer, programFileName);
                }
                _cp5200.Playbill_SaveToFile(playBillPointer, GeneratePlayBillFileName(scheduleName));
                _cp5200.DestroyProgram(playBillPointer);
            }

        }

        private void SaveImageToFile(string sCounter, ImageSelect image)
        {
            string tempFileName =
                HttpContext.Current.Server.MapPath(string.Concat(ImageDirectory, sCounter, ImageExtension));
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(image.ImageUrl, tempFileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
            }
        }

        private string GeneratePlayBillFileName(string scheduleName)
        {
            return PlaybillFileName = HttpContext.Current.Server.MapPath(string.Concat(ProgramFileDirectory, StripCharacters.Strip(scheduleName).Substring(8), PlaybillFileExtension));
        }

        private string GenerateProgramFileName(string sCounter)
        {
            return HttpContext.Current.Server.MapPath(string.Concat(ProgramFileDirectory, sCounter, ProgramFileExtension));
        }
    }
}



