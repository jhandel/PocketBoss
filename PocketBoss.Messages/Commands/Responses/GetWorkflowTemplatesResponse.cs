using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PocketBoss.Common.Messaging;
using PocketBoss.Models;

namespace PocketBoss.Messages.Commands.Responses
{
    [Serializable]
    public class GetWorkflowTemplatesResponse : MessageBase
    {
         [Serializable]
        public class WorkflowTemplateSummary {
            public long WorkflowTemplateId { get; set; }
            public string WorkflowTemplateName { get; set; }
            public long OpenWorkflowsRunning { get; set; }
            public long WorkflowsCompleted { get; set; }
            public string WorkflowTemplateDescription { get; set; }
         }
         public List<WorkflowTemplateSummary> WorkflowTemplates { get; set; }
    }

    [Serializable]
    public class GetSingleWorkflowTemplateResponse : MessageBase
    {
        public WorkflowTemplate WorkflowTemplate { get; set; }
    }
}
