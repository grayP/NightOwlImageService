using nightowlsign.data.Models.Stores;
using NightOwlImageService.AutoFac;
using NightOwlImageService.Services;
using Topshelf;
using Topshelf.Autofac;
using Autofac;

namespace NightOwlImageService
{
    public class Program
    {
        public static void Main()
        {
            var container = Ioc.LetThereBeIoc();

            HostFactory.Run(x => //1
            {
                x.UseAutofacContainer(container);

                x.Service<ServiceRunner>(s =>                        //2
                {
                    s.ConstructUsing(() =>container.Resolve<ServiceRunner>());     //3
                    s.WhenStarted(tc => tc.Start());              //4
                    s.WhenStopped(tc => tc.Stop());               //5
                });
                x.RunAsLocalSystem();                            //6

                x.SetDescription($"Send Images to NightOwlSigns {System.Reflection.Assembly.GetExecutingAssembly()}");        //7
                x.SetDisplayName("NightOwlImageSend");
                x.SetServiceName("NightOwlImageSend");
            });
        }
    }
}
