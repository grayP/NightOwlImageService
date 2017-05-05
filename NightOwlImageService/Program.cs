using NightOwlImageService.AutoFac;
using NightOwlImageService.Services;
using Topshelf;


namespace NightOwlImageService
{
    public class Program
    {
        public static void Main()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

            var container = ioc.LetThereBeIoc();

            HostFactory.Run(x => //1
            {
                x.UseAutofacContainer(container);

                x.Service<ServiceRunner>(s =>                        //2
                {
                    s.ConstructUsing(name => new ServiceRunner(assembly));     //3
                    s.WhenStarted(tc => tc.Start());              //4
                    s.WhenStopped(tc => tc.Stop());               //5
                });
                x.RunAsLocalSystem();                            //6

                x.SetDescription($"Send Images to NightOwlSigns {assembly}");        //7
                x.SetDisplayName("NightOwlImageSend");
                x.SetServiceName("NightOwlImageSend");
            });
        }
    }
}
