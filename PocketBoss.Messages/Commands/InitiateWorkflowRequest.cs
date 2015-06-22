
using PocketBoss.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBoss.Messages.Commands
{
    [Serializable]
    public class InitiateWorkflowRequest : MessageBase
    {
        public string TargetObjectId { get; set; }
        public string TargetObjectType { get; set; }
        public string WorkflowTemplateName { get; set; }
    }
}
