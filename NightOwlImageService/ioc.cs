using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Builder;
using ConfigInjector.Configuration;
using ImageProcessor.Services;

namespace NightOwlImageService
{
   public static class ioc
    {
        public static IContainer LetThereBeIOC()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<SendCommunicator>();

            ConfigurationConfigurator.RegisterConfigurationSettings()
                                     .FromAssemblies(typeof(SendCommunicator).Assembly)
                                     .RegisterWithContainer(configSetting => builder.RegisterInstance(configSetting)
                                                                                    .AsSelf()
                                                                                    .SingleInstance())
                                     .DoYourThing();

            return builder.Build();
        }
    }
}
