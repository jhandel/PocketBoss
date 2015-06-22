using PocketBoss.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBoss.Messages.Commands
{
    public class RecordStateAction: MessageBase
    {
        public string TargetObjectId { get; set; }
        public string TargetObjectType { get; set; }
        public long WorkflowInstanceId { get; set; }
        public string WorkflowTemplateName { get; set; }
        public string State { get; set; }
        public long StateId { get; set; }
        public long TaskId { get; set; }
        public string Event { get; set; }
    }
}
