using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBoss.Common.Messaging
{
    [Serializable]
    public abstract class MessageBase
    {
        public Guid CorrelationId { get; set; }
        public Guid TenantId { get; set; }
        public string AuditContext { get; set; }
    }
}
