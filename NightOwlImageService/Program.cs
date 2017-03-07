using System;
using System.Timers;
using NightOwlImageService.Services;
using Topshelf;


namespace NightOwlImageService
{
    public class Program
    {
       public  static void Main()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

            HostFactory.Run(x =>                                 //1
            {
                x.Service<ServiceRunner>(s =>                        //2
                {
                    s.ConstructUsing(name => new ServiceRunner());     //3
                    s.WhenStarted(tc => tc.Start());              //4
                    s.WhenStopped(tc => tc.Stop());               //5
                });
                x.RunAsLocalSystem();                            //6

                x.SetDescription( $"Send Images to NightOwlSigns {assembly}");        //7
                x.SetDisplayName("NightOwlImageSend");                       
                x.SetServiceName("NightOwlImageSend");                       
            });
        }
    }
}
