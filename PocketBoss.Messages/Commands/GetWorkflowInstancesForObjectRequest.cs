using PocketBoss.Common;
using PocketBoss.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBoss.Messages.Commands
{
    [Serializable]
    public class GetWorkflowInstancesForObjectRequest:MessageBase
    {
        public long TargetObjectId { get; set; }
        public string TargetObjectType { get; set; }
        public string WorkflowTemplateName { get; set; }
        public bool IncludeFinished { get; set; }
    }
 
}
