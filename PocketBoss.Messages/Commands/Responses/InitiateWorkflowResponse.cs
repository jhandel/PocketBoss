
using PocketBoss.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBoss.Messages.Commands.Responses
{
    [Serializable]
    public class InitiateWorkflowResponse : MessageBase
    {
        public long WorkflowInstanceId { get; set; }
    }
}
