using PocketBoss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PocketBoss.Common.Messaging;

namespace PocketBoss.Messages.Events
{
    [Serializable]
    public class WorkflowStateNotification : MessageBase
    {
        public string TargetObjectId { get; set; }
        public string TargetObjectType { get; set; }
        public long WorkflowInstanceId { get; set; }
        public string WorkflowTemplateName { get; set; }
        public string State { get; set; }
        public long StateId { get; set; }
        public bool IsComplete { get; set; } 
    }
    [Serializable]
    public class WorkflowTaskNotification : MessageBase
    {
        public string TargetObjectId { get; set; }
        public string TargetObjectType { get; set; }
        public long WorkflowInstanceId { get; set; }
        public string WorkflowTemplateName { get; set; }
        public string State { get; set; }
        public string Task { get; set; }
        public long StateId { get; set; }
        public long TaskId { get; set; }
        public bool IsComplete { get; set; }
    }
}
