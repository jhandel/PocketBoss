using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBoss.Common.Messaging
{
    class ObjectUpdatedNotification:MessageBase
    {
        public string ObjectType { get; set; }
        public string ObjectId { get; set; }
    }
}
