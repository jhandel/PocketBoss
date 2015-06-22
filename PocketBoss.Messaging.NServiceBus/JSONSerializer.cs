using NServiceBus.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBoss.Messaging.NServiceBus
{
    class JSONSerializer : IMessageSerializer
    {
        public void Serialize(object message, Stream stream)
        {
            // Add code to serialize message
            throw new NotImplementedException();
        }

        public object[] Deserialize(Stream stream, IList<Type> messageTypes = null)
        {
            // Add code to deserialize message
            throw new NotImplementedException();
        }

        public string ContentType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
