using ScreenBrightness.cp5200;
using System;
using System.Net.Sockets;

namespace ScreenBrightness
{
    public class ScreenBrightnessSetter : IBrightness
    {
        const string ReadData = "FFFFFFFF08000000683201460101E300";
        private const string precede = "FFFFFFFF200000006832FF460100";
        private const string setBrightness = "0000000000000000000000000000000000000000000000000000";
        private string _ipAddress;
        private int _brightnessLevel;
        private string _port;
        public ScreenBrightnessSetter(string IpAddress, string port, int brightnessLevel)
        {
            _ipAddress = IpAddress;
            _port = port;
            _brightnessLevel = brightnessLevel;
        }

        public void SetBrightness()
        {
            Connect(_ipAddress, _port, _brightnessLevel);
            // Connect("192.168.0.222", ReadData);
            var success = RestartSign();
            //var answer=  testByte(preCede, setBrightness, 30);
        }

        private static void Connect(string IpAddress, string port, int brightnessLevel)
        {
            try
            {
                int iPort;
                if (int.TryParse(port, out iPort))
                {

                    using (var client = new TcpClient(IpAddress, iPort))
                    {
                        Byte[] command = ConstructCommand.ConstructBrightnessCommand($"{precede}{setBrightness}", brightnessLevel);
                        using (var stream = client.GetStream())
                        {
                            // NetworkStream stream = client.GetStream();
                            // Send the message to the connected TcpServer. 
                            stream.Write(command, 0, command.Length);
                            // Buffer to store the response bytes.
                            var response = new Byte[256];
                            // String to store the response ASCII representation.
                            var responseData = string.Empty;
                            // Read the first batch of the TcpServer response bytes.
                            Int32 bytes = stream.Read(response, 0, response.Length);
                            responseData = System.Text.Encoding.ASCII.GetString(response, 0, bytes);
                        }
                    }
                    //stream.Close();
                    //client.Close();
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }

        public static int RestartSign()
        {
            var success = Cp5200External.CP5200_Net_RestartApp(Convert.ToByte(1));
            Console.WriteLine($"Restart of sign - {success}", "Result");
            return success;
        }

    }
}
