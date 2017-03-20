using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Builder;
using ConfigInjector;
using ConfigInjector.Configuration;
using ImageProcessor.Services;
using Logger;
using nightowlsign.data.Models.SendToSign;
using nightowlsign.data.Models.StoreScheduleLog;
using NightOwlImageService.Configuration;
using NightOwlImageService.Services;

namespace NightOwlImageService
{
   public static class ioc
    {
        public static IContainer LetThereBeIoc()
        {

            var builder = new ContainerBuilder();
            builder.RegisterType<ServiceRunner>().InstancePerLifetimeScope();
            builder.RegisterType<SendCommunicator>().As<ISendCommunicator>().InstancePerLifetimeScope();
            builder.RegisterType<SendToSignManager>().As<ISendToSignManager>().InstancePerLifetimeScope();
            builder.RegisterType<CreateFilesToSend>().As<ICreateFilesToSend>().InstancePerLifetimeScope();
            builder.RegisterType<StoreScheduleLogManager>().As<IStoreScheduleLogManager>().InstancePerLifetimeScope();
            builder.RegisterType<MLogger>().As<IMLogger>().InstancePerLifetimeScope();
        

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
