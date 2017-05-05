using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ImageProcessor.Services;
using Logger;
using Logger.Logger;
using nightowlsign.data.Models;
using nightowlsign.data.Models.Stores;
using nightowlsign.data.Models.StoreScheduleLog;
using NightOwlImageService.Services;

namespace NightOwlImageService.AutoFac
{
    public static class ioc
    {
        public static IContainer LetThereBeIoc()
        {

            var builder = new ContainerBuilder();
            builder.RegisterType<ServiceRunner>().InstancePerLifetimeScope();
            builder.RegisterType<SendCommunicator>().As<ISendCommunicator>().InstancePerLifetimeScope();
            builder.RegisterType<MLogger>().As<IMLogger>().InstancePerLifetimeScope();
           // builder.RegisterType<SendToSignManager>().As<ISendToSignManager>().InstancePerLifetimeScope();
           // builder.RegisterType<ImageManager>().As<IImageManager>().InstancePerLifetimeScope();
          //  builder.RegisterType<StoreManager>().As<IStoreManager>().InstancePerLifetimeScope();
          //  builder.RegisterType<StoreScheduleLogManager>().As<IStoreScheduleLogManager>().InstancePerLifetimeScope();
           // builder.RegisterType<StoreViewModel>().As<IStoreViewModel>().InstancePerLifetimeScope();


            //ConfigurationConfigurator.RegisterConfigurationSettings()
            //                         .FromAssemblies(typeof(NightOwlImageService).Assembly)
            //                         .RegisterWithContainer(configSetting => builder.RegisterInstance(configSetting)
            //                                                                        .AsSelf()
            //                                                                        .SingleInstance())
            //                         .DoYourThing();

            return builder.Build();

        }
    }
}
