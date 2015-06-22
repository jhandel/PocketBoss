using NServiceBus;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;
using NServiceBus.Features;
using NServiceBus.Newtonsoft.Json;
using PocketBoss.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PocketBoss.Messaging.NServiceBus
{
    public class NServiceBusService : IMessagingService
    {
        private IBus _bus;

        public NServiceBusService()
        {
        }

        internal NServiceBusService(IBus bus)
        {
            _bus = bus;
        }
        public void OpenConnection(IDictionary<string, string> connectionDetails)
        {
            BusConfiguration busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName(connectionDetails["EndpointName"]);
            busConfiguration.UseSerialization<NewtonsoftSerializer>();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.EnableInstallers();
            busConfiguration.DisableFeature<AutoSubscribe>();


            List<Assembly> assemblies = new List<Assembly>();
            assemblies.Add(Assembly.LoadFrom("PocketBoss.Messages.dll"));
            assemblies.Add(Assembly.LoadFrom("NServiceBus.Newtonsoft.Json.dll"));
            if (connectionDetails.ContainsKey("HandlersAssembly"))
            {
                assemblies.Add(Assembly.LoadFrom(connectionDetails["HandlersAssembly"]));

            }
            else
            {
                assemblies.Add(Assembly.GetExecutingAssembly());
            }


            busConfiguration.AssembliesToScan(assemblies);

            ConventionsBuilder conventions = busConfiguration.Conventions();
            conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("PocketBoss.Messages") && t.Namespace.EndsWith("Commands"));
            conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("PocketBoss.Messages") && t.Namespace.EndsWith("Events"));
            conventions.DefiningMessagesAs(t => t.Namespace != null && t.Namespace.StartsWith("PocketBoss.Messages"));

            if (_bus != null)
            {
                CloseConnection();
            }
            _bus = Bus.Create(busConfiguration).Start();
        }

        public void CloseConnection()
        {
            if (_bus != null)
            {
                _bus.Dispose();
                _bus = null;
            }
        }

        public void AddSubscription<T>()
        {
            _bus.Subscribe<T>();
        }

        public void RemoveSubscription<T>()
        {
            _bus.Unsubscribe<T>();
        }

        public void Dispose()
        {
            CloseConnection();
        }

        public void Send<T>(T message)
        {
            _bus.Send(message);
        }

        public void Send<T>(string endpoint, T message)
        {
            _bus.Send(endpoint, message);
        }


        public async Task<R> Send<T, R>(T message) where R: class, new()
        {
            R returnMessage = new R();
            await _bus.Send(message).Register((cr) => {
                if (cr.Messages.Count() > 0)
                {
                    returnMessage = cr.Messages[0] as R;
                }
            });
            return returnMessage;
        }

        public async Task<R> Send<T, R>(string endpoint, T message) where R : class, new()
        {
            R returnMessage = new R();
            await _bus.Send(endpoint, message).Register((cr) =>
            {
                if (cr.Messages.Count() > 0)
                {
                    returnMessage = cr.Messages[0] as R;
                }
            });
            return returnMessage;
        }

        public void Reply<T>(T message, int statusCode = 1)
        {
            _bus.Reply(message);
            _bus.Return(statusCode);
        }

        public void Publish<T>(T message)
        {
            _bus.Publish<T>(message);
        }
    }
}
