using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBoss.Common
{
    public class IoCContext
    {
        private static readonly Lazy<IoCContext> lazy =
             new Lazy<IoCContext>(() => new IoCContext());

        public UnityContainer Container { get; set; }

        public static IoCContext Current { get { return lazy.Value; } }

        private IoCContext()
        {
            Container = new UnityContainer();
            Container.LoadConfiguration();
        }
    }
}
