using ScreenBrightness.cp5200;
using System;
using System.Net.Sockets;

namespace ScreenBrightness
{
    class Program
    {
        const string ReadData = "FFFFFFFF08000000683201460101E300";
        private const string preCede = "FFFFFFFF200000006832FF460100";
        const string setBrightness = "0000000000000000000000000000000000000000000000000000";


        static void Main(string[] args)
        {
            Connect("192.168.0.222", preCede, setBrightness, 1);
            // Connect("192.168.0.222", ReadData);
            var success = RestartSign();
            //var answer=  testByte(preCede, setBrightness, 30);
        }

        static void Connect(string server, string precede, string message, int brightnessLevel)
        {
            try
            {
                // combination.
                var port = 5200;
                //var client = new TcpClient(server, port);
                using (var client = new TcpClient(server, port))
                {
                    // Translate the passed message into ASCII and store it as a Byte array.
                    Byte[] command = ConstructCommand.ConstructBrightnessCommand($"{precede}{message}", brightnessLevel);

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
