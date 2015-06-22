using PocketBoss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PocketBoss.Common.Messaging;
using PocketBoss.Models;

namespace PocketBoss.Messages.Commands
{
    public class SaveWorkflowTemplateRequest:MessageBase
    {
        public WorkflowTemplate WorkflowTemplate { get; set; }
    }
}
