using PocketBoss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PocketBoss.Common.Messaging;
using PocketBoss.Models;

namespace PocketBoss.Messages.Commands.Responses
{
    public class SaveWorkflowTemplateResponse : MessageBase
    {
        public WorkflowTemplate WorkflowTemplate { get; set; }
    }
}
