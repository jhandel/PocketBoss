using PocketBoss.Messages;
using PocketBoss.Common.Messaging;
using PocketBoss.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBoss.Messages.Commands
{
    [Serializable]
    public class GetWorkflowInstanceDetailsRequest:MessageBase
    {
        public string TargetObjectId { get; set; }
        public string TargetObjectType { get; set; }
        public long WorkflowInstanceId { get; set; }
    }
}
