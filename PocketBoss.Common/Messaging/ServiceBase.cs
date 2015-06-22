using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace PocketBoss.Common.Messaging
{
    public abstract class TopshelfMassTransitServiceBase : ServiceControl
    {
        public TopshelfMassTransitServiceBase()
        {
        }

        public bool Start(HostControl hostControl)
        {
            throw new NotImplementedException();
        }

        public bool Stop(HostControl hostControl)
        {
            throw new NotImplementedException();
        }
    }
}
