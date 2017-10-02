using Autofac;
using ImageProcessor.Services;
using Logger.Service;
using NightOwlImageService.Services;
using nightowlsign.data;
using nightowlsign.data.Models.Logging;
using nightowlsign.data.Models.SendToSign;
using nightowlsign.data.Models.StoreScheduleLog;
using nightowlsign.data.Models.UpLoadLog;
using ScreenBrightness;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NightOwlImageService.AutoFac
{
    public static class Ioc
    {
        public static IContainer LetThereBeIoc()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ServiceRunner>();
           // builder.RegisterType<SendCommunicator>().As<ISendCommunicator>();
            builder.RegisterType<nightowlsign_Entities>().As<Inightowlsign_Entities>().InstancePerDependency();
           // builder.RegisterType<SendToSignManager>().As<ISendToSignManager>();
           // builder.RegisterType<ScreenImageManager>().As<IScreenImageManager>().InstancePerDependency();
           // builder.RegisterType<StoreManager>().As<IStoreManager>().InstancePerLifetimeScope();
            builder.RegisterType<StoreScheduleLogManager>().As<IStoreScheduleLogManager>();
            //builder.RegisterType<StoreViewModel>().As<IStoreViewModel>().InstancePerRequest();
           // builder.RegisterType<Assembly>().As<_Assembly>().InstancePerLifetimeScope();
            //builder.RegisterType<GeneralLogger>().As<IGeneralLogger>();
            //builder.RegisterType<LoggingManager>().As<ILoggingManager>();
            //builder.RegisterType<UpLoadLogger>().As<IUpLoadLogger>();
            //builder.RegisterType<UpLoadLoggingManager>().As<IUpLoadLoggingManager>();
           // builder.RegisterType<ScreenBrightnessSetter>().As<IBrightness>().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}
