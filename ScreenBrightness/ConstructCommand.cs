using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace ScreenBrightness
{
    public class ConstructCommand
    {
        public byte[] Command { get; set; }

        public ConstructCommand()
        {
            Command = new byte[40];
        }
        internal void SetupCommandStart()
        {
            Command[0] = Convert.ToByte("FF");
            Command[1] = Convert.ToByte("FF");
            Command[2] = Convert.ToByte("FF");
            Command[3] = Convert.ToByte("FF");
            Command[4] = Convert.ToByte("20");
            Command[5] = Convert.ToByte("00");
            Command[6] = Convert.ToByte("00");
            Command[7] = Convert.ToByte("00");
            Command[8] = Convert.ToByte("68");
            Command[9] = Convert.ToByte("32");
            Command[10] = Convert.ToByte("FF");
            Command[11] = Convert.ToByte("46");
            Command[12] = Convert.ToByte("01");
            Command[13] = Convert.ToByte("00");
        }
        public static byte[] ConstructBrightnessCommand(string hex, int level)
        {
            byte[] newCommand = new byte[40];
            var commandArray = StringToByteArray(hex);
            SetNewLevel(commandArray, level);
            SetCheckSum(commandArray);
            return commandArray;
        }

        private static void SetCheckSum(byte[] commandArray)
        {
            var sum = commandArray.Skip(8).Take(24).Sum(x => Convert.ToInt16(x));
            //for (var i = 8; i <= 37; i++)
            //{
            //    sum += Convert.ToInt16(commandArray[i]);
            //}
            var hexValue = $"(0000{sum.ToString("x")}";
            hexValue = hexValue.Substring(Math.Max(0, hexValue.Length - 4)); ;
            var checkSum = StringToByteArray(hexValue);
            commandArray[38] = checkSum[1];
            commandArray[39] = checkSum[0];
        }

        internal void SetLevel(int index, int level)
        {
            Command[index] = Convert.ToByte(level);
        }

        private static void SetNewLevel(byte[] commandArray, int level)
        {
            for (var i = 14; i <= 37; i++)
            {
                commandArray[i] = Convert.ToByte(level);
            }
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}
