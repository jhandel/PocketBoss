using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PocketBoss.Messages;
using NServiceBus;
using PocketBoss.Common;
using PocketBoss.Common.Messaging;
using Microsoft.Practices.Unity;
using NServiceBus.Config.ConfigurationSource;
using NServiceBus.Config;
using PocketBoss.Messages.Events;
using PocketBoss.Messages.Commands;

namespace PocketBoss.TestingCLI.Workflow
{
    class WorkflowStateNotificationHandler : IHandleMessages<WorkflowStateNotification>
    {
        public void Handle(WorkflowStateNotification message)
        {
            IMessagingService bus = IoCContext.Current.Container.Resolve<IMessagingService>();
            System.Console.WriteLine("State Updated("+message.WorkflowInstanceId+"): " + message.State);
            if (message.IsComplete)
            {
                System.Console.WriteLine(" --- Workflow " + message.WorkflowInstanceId + " finished --- ");
            }
        }   
    }

    class WorkflowTaskNotificationHandler : IHandleMessages<WorkflowTaskNotification>
    {
        public void Handle(WorkflowTaskNotification message)
        {
            IMessagingService bus = IoCContext.Current.Container.Resolve<IMessagingService>();
            System.Console.WriteLine("Task Requested(" + message.WorkflowInstanceId + "): " + message.Task);
            var workflowTaskEventNotification = new RecordTaskAction()
            {
                AuditContext = "Console App",
                CorrelationId = Guid.NewGuid(),
                StateId = message.StateId,
                TaskId = message.TaskId,
                TenantId = message.TenantId,
                TargetObjectId = message.TargetObjectId,
                TargetObjectType = message.TargetObjectType,
                WorkflowInstanceId = message.WorkflowInstanceId,
                WorkflowTemplateName = message.WorkflowTemplateName,
            };
            bool send = false;
            
            switch (message.Task)
            {
                case "CanAccessAd":
                    workflowTaskEventNotification.Event = "AD_Online";
                    send = true;
                    break;
                case "CheckUserName":
                    workflowTaskEventNotification.Event = "User_Found";
                    send = true;
                    break;
            }
            if (send)
            {
                Random r = new Random();
                //System.Threading.Thread.Sleep(r.Next(1000));
                bus.Send<RecordTaskAction>(workflowTaskEventNotification);
            }

            if (message.IsComplete)
            {
                System.Console.WriteLine(" --- Task Chain for workflow " + message.WorkflowInstanceId + " Completed --- ");
            }
        }
    }
}
