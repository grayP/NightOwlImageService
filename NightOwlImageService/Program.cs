using System;
using Autofac;
using ConfigInjector;
using ConfigInjector.Configuration;
using ImageProcessor.Services;
using Logger;
using nightowlsign.data.Models.SendToSign;
using nightowlsign.data.Models.StoreScheduleLog;
using NightOwlImageService.Services;
using Topshelf;
using Topshelf.Autofac;


namespace NightOwlImageService
{
    public class Program
    {
        public static void Main(string[] args)
        {
             var container = ioc.LetThereBeIoc();

            HostFactory.Run(x => //1
            {
                x.UseAutofacContainer(container);

               x.Service<ServiceRunner>(s => 
                {
                    s.ConstructUsingAutofacContainer(); //3
                    s.WhenStarted(service => service.Start()); //4
                    s.WhenStopped(service => service.Stop()); //5
                });
                //6

                x.SetDescription($"Send Images to NightOwlSigns"); //7
                x.SetDisplayName("NightOwlImageSend");
                x.SetServiceName("NightOwlImageSend");
            });
        }
    }
}
