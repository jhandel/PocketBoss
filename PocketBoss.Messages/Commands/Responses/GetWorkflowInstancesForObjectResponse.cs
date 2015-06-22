using PocketBoss.Common;
using PocketBoss.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBoss.Messages.Commands.Responses
{
    [Serializable]
    public class GetWorkflowInstancesForObjectResponse : MessageBase
    {
        [Serializable]
        public class WorkflowStatusOverview
        {
            public long TargetObjectId { get; set; }
            public string TargetObjectType { get; set; }
            public long WorkflowInstanceId { get; set; }
            public string WorkflowTemplateName { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public DateTime LastStateTranstion { get; set; }
            public bool Completed { get; set; }
            public string CurrentState { get; set; }
            public string[] OutstandingTasks { get; set; }
        }
        public List<WorkflowStatusOverview> Workflows { get; set; }
    }
}
