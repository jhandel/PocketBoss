using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PocketBoss.Common.Messaging;
using PocketBoss.Models;

namespace PocketBoss.Messages.Commands
{
    [Serializable]
    public class GetWorkflowTemplatesRequest: MessageBase
    {
        
        public string TargetObjectType { get; set; }
    }

    [Serializable]
    public class GetSingleWorkflowTemplateRequest : MessageBase
    {
        public long? WorkflowTemplateId { get; set; }
        public string WorkflowTemplateName { get; set; }
    }

}
