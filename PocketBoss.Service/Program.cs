
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.Logging;
using Topshelf.Runtime;


namespace PocketBoss.Service
{

    public class Program
    {
        public static void Main()
        {

            HostFactory.Run(x =>
            {

                x.AfterInstall(() =>
                {
                });

                x.RunAsLocalSystem();

                x.SetDescription("Manages workflow progression for any object");
                x.SetDisplayName("PocketBoss Workflow Service");
                x.SetServiceName("PocketBoss.Service");

                x.Service(CreateService);

            });
            System.Console.ReadLine();
        }

        static Service CreateService(HostSettings hostSettings)
        {
            return new Service();
        }
    }

}
