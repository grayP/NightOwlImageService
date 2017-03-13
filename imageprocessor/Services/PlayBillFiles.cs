using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ImageProcessor.CP5200;
using ImageProcessor.Enums;
using Logger;


namespace ImageProcessor.Services
{
    public class PlayBillFiles
    {
        private readonly int _screenWidth;
        private readonly int _screenHeight;
        private readonly int _displayTime;
        private readonly byte _colourMode;
        private int _playWindowNumber;
        private mLogger _logger;
        public PlayBillFiles(int width, int height, int displayTime, byte colourMode, mLogger logger)
        {
            _screenWidth = width;
            _screenHeight = height;
            _displayTime = displayTime;
            _colourMode = colourMode;
            _logger = logger;
        }

        public IntPtr Program_Create()
        {
            try
            {
                return Cp5200External.CP5200_Program_Create(Convert.ToInt32(_screenWidth), Convert.ToInt32(_screenHeight), 0x77);
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"PlayBillFiles - Program_create - Program create threw an error: {ex.Message}");
                return IntPtr.Zero;
            }
        }

        public int AddPlayWindow(IntPtr programPointer)
        {
            if (Program_SetProperty(programPointer, 0xFFFF, 1) > 0)
              //  if (Program_SetProperty(programPointer, _displayTime, (uint)Set_Program_Property.ProgramPlayTime) > 0)
                {
                    _playWindowNumber = Program_AddPlayWindow(programPointer);
                return _playWindowNumber;
            }
            return -1;
        }

        public int Program_SetProperty(IntPtr programPointer, int propertyValue, uint propertyId)
        {
            //1: program repetition play times
            //2: program play time
            //3: Code conversion
            return Cp5200External.CP5200_Program_SetProperty(programPointer, propertyValue, propertyId);
        }

        public int Program_AddPlayWindow(IntPtr programPointer)
        {
            ushort xStart = 0;
            ushort yStart = 0;
            var result = Cp5200External.CP5200_Program_AddPlayWindow(programPointer, xStart, yStart, _screenWidth, _screenHeight);

            return result;
        }
        public int Program_AddPicture(IntPtr programPointer, string path, int mode, int effect, int speed, int stayTime, int compress)
        {
            return Cp5200External.CP5200_Program_AddPicture(programPointer, _playWindowNumber, path, mode, effect, speed, stayTime, compress);
        }

        public int Program_Add_Image(IntPtr programPointer, int windowNo, IntPtr picturePointer, int nMode, int nEffect, int nSpeed, int nStay, int nCompress)
        {
            return Cp5200External.CP5200_Program_AddPicture(programPointer, windowNo, picturePointer, nMode, nEffect,
                nSpeed, nStay, nCompress);
        }

        public int Program_SaveFile(IntPtr programPointer, string filePathAndName)
        {
            System.IO.File.Delete(filePathAndName);
            try
            {
                return Cp5200External.CP5200_Program_SaveToFile(programPointer, filePathAndName);

            }
            catch (Exception ex)
            {
                _logger.WriteLog($"PlayBillFiles - SaveFile - Program save threw error; {ex.Message}");
                throw;
            }
        }

        //Now move on to playbill files
        public IntPtr playBill_Create()
        {
            try
            {
                return Cp5200External.CP5200_Playbill_Create(_screenWidth, _screenHeight, 0x77);
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"PlayBillFiles - playBill_Create - PlayBill create threw an error: {ex.Message}");
                return IntPtr.Zero;
            }
        }
        public int Playbill_SetProperty(IntPtr playBillPointer, ushort propertyValue, uint propertyId)
        {
            //1:Rotate 90 degrees
            return Cp5200External.CP5200_Playbill_SetProperty(playBillPointer, propertyValue, propertyId);
        }

        public int Playbill_AddFile(IntPtr playBillPointer, string path)
        {
            return Cp5200External.CP5200_Playbill_AddFile(playBillPointer, Marshal.StringToHGlobalAnsi(path));
        }

        public int Playbill_SaveToFile(IntPtr playBillPointer, string filePathAndName)
        {
            try
            {
                System.IO.File.Delete(filePathAndName);
                return Cp5200External.CP5200_Playbill_SaveToFile(playBillPointer, filePathAndName);
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"playBillFiles - SavetoFile - Program save to file threw error: {ex.Message}");
                throw;
            }
        }

        internal void DestroyProgram(IntPtr programPointer)
        {
            Cp5200External.CP5200_Program_Destroy(programPointer);
        }
    }
}
