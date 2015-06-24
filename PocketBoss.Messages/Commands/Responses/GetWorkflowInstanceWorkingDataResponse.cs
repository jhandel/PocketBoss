using PocketBoss.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBoss.Messages.Commands
{
    [Serializable]
    public class GetWorkflowInstanceWorkingDataResponse : MessageBase
    {
        public string TargetObjectId { get; set; }
        public string TargetObjectType { get; set; }
        public long WorkflowInstanceId { get; set; }
        public Dictionary<string, string> WorkingData{ get; set; }
    }
}
