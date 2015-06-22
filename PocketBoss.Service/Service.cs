using PocketBoss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Microsoft.Practices.Unity;
using PocketBoss.Common.Messaging;
using PocketBoss.Messages;
using PocketBoss.Messages.Events;

namespace PocketBoss.Service
{
    public class Service : ServiceControl
    {
        IMessagingService _bus;
        public Service()
            : base()
        {

        }
        public bool Start(HostControl hostControl)
        {
            _bus = IoCContext.Current.Container.Resolve<IMessagingService>("WorkflowMessaging");
            IDictionary<string, string> settings = new Dictionary<string, string>();
            settings.Add("EndpointName", "PocketBoss_Local_Service");
            _bus.OpenConnection(settings);
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            if (_bus != null)
            {
                _bus.Dispose();
            }
            return true;
        }
    }
}
