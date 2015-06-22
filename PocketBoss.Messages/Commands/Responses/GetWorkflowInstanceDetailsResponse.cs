using PocketBoss.Messages;
using PocketBoss.Common.Messaging;
using PocketBoss.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBoss.Messages.Commands.Responses
{
    [Serializable]
    public class GetWorkflowInstanceDetailsResponse : MessageBase
    {
        public string TargetObjectId { get; set; }
        public string TargetObjectType { get; set; }
        public WorkflowInstance WorkFlowInstance { get; set; }
    }
}
