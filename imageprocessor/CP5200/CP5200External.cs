using ImageProcessor.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;
using Serilog;

namespace ImageProcessor.CP5200
{
    public class Cp5200External
    {
        private readonly ushort _screenWidth;
        private readonly ushort _screenHeight;
        private readonly ushort _displayTime;
        private readonly byte _colourMode;

        private const string DllPath = "CP5200.dll";

        [DllImport(DllPath, CharSet = CharSet.Auto)]
        public static extern IntPtr CP5200_Program_Create(int width, int height, byte color);

        [DllImport(DllPath, CharSet = CharSet.Auto)]
        public static extern ushort CP5200_Program_SetProperty(IntPtr hObj, int nPropertyValue, uint nPropertyId);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        public static extern int CP5200_Program_AddPicture(IntPtr hObj, int nWinNo,
            [MarshalAs(UnmanagedType.LPStr)] string pPictFile, int nMode, int nEffect, int nSpeed, int nStay,
            int nCompress);

        [DllImport(DllPath, CharSet = CharSet.Auto)]
        public static extern int CP5200_Program_AddPicture(IntPtr hObj, int nWinNo, IntPtr pPictFile, int nMode, int nEffect, int nSpeed, int nStay, int nCompress);


        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        public static extern int CP5200_Program_AddPlayWindow(IntPtr hobj, int x, int y, int cx, int cy);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        public static extern int CP5200_Program_SetWindowProperty(IntPtr hobj, int nWinNo, int nPropertyValue,
            int nPropertyId);


        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int CP5200_Program_SaveToFile(IntPtr hObj,
            [MarshalAs(UnmanagedType.LPStr)] string pFileName);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        public static extern int CP5200_Program_Destroy(IntPtr hObj);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr CP5200_Playbill_Create(int width, int height, byte colour);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        public static extern ushort CP5200_Playbill_SetProperty(IntPtr hObj, ushort nPropertyValue, uint nPropertyId);


      //  [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
       // public static extern int CP5200_Playbill_AddFile(IntPtr hObj, [MarshalAs(UnmanagedType.LPStr)] string pFilename);

        [DllImport(DllPath, CharSet = CharSet.Auto)]
        public static extern int CP5200_Playbill_AddFile(IntPtr hObj, IntPtr pFilename);


        [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int CP5200_Playbill_SaveToFile(IntPtr hObj,
           [MarshalAs(UnmanagedType.LPStr)] string pFileName);


        //Network 
        [DllImport(DllPath, CharSet = CharSet.Auto)]
        public static extern int CP5200_Net_Init(uint dwIP, int nIPPort, uint dwIDCode, int nTimeOut);


        [DllImport(DllPath, CharSet = CharSet.Auto)]
        public static extern int CP5200_Net_UploadFile(int nCardID, IntPtr pSourceFilename, IntPtr pTargetFilename);

        [DllImport(DllPath, CharSet = CharSet.Auto)]
        public static extern int CP5200_Net_RestartApp(byte nCardID);


        public Cp5200External(ushort width, ushort height, ushort displayTime, byte colourMode)
        {
            _screenWidth = width;
            _screenHeight = height;
            _displayTime = displayTime;
            _colourMode = colourMode;
        }
    }
 }

