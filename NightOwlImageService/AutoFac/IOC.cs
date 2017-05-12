using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ImageProcessor.Services;
using Logger;
using Logger.Service;
using nightowlsign.data;
using nightowlsign.data.Models;
using nightowlsign.data.Models.Logging;
using nightowlsign.data.Models.SendToSign;
using nightowlsign.data.Models.Stores;
using nightowlsign.data.Models.StoreScheduleLog;
using nightowlsign.data.Models.UpLoadLog;
using NightOwlImageService.Services;

namespace NightOwlImageService.AutoFac
{
    public static class Ioc
    {
        public static IContainer LetThereBeIoc()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ServiceRunner>().InstancePerLifetimeScope();
            builder.RegisterType<SendCommunicator>().As<ISendCommunicator>().InstancePerLifetimeScope();
            builder.RegisterType<nightowlsign_Entities>().As<Inightowlsign_Entities>().InstancePerLifetimeScope();
            builder.RegisterType<SendToSignManager>().As<ISendToSignManager>().InstancePerLifetimeScope();
            builder.RegisterType<ScreenImageManager>().As<IScreenImageManager>().InstancePerLifetimeScope();
            builder.RegisterType<StoreManager>().As<IStoreManager>().InstancePerLifetimeScope();
            builder.RegisterType<StoreScheduleLogManager>().As<IStoreScheduleLogManager>().InstancePerLifetimeScope();
            builder.RegisterType<StoreViewModel>().As<IStoreViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<Assembly>().As<_Assembly>().InstancePerLifetimeScope();
            builder.RegisterType<GeneralLogger>().As<IGeneralLogger>().InstancePerLifetimeScope();
            builder.RegisterType<LoggingManager>().As<ILoggingManager>().InstancePerLifetimeScope();
            builder.RegisterType<UpLoadLogger>().As<IUpLoadLogger>().InstancePerLifetimeScope();
            builder.RegisterType<UpLoadLoggingManager>().As<IUpLoadLoggingManager>().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}
