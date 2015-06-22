using PocketBoss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PocketBoss.Messages;
using PocketBoss.Models;
using PocketBoss.Common.Messaging;
using Microsoft.Practices.Unity;
using PocketBoss.Messages.Events;
using PocketBoss.Messages.Commands;
using PocketBoss.Messages.Commands.Responses;

namespace PocketBoss.TestingCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            IMessagingService bus = IoCContext.Current.Container.Resolve<IMessagingService>();
            IDictionary<string, string> settings = new Dictionary<string, string>();
            settings.Add("EndpointName", "PocketBoss_Test_Client");
            settings.Add("HandlersAssembly", "PocketBoss.TestingCLI.exe");
            bus.OpenConnection(settings);
            bus.AddSubscription<WorkflowStateNotification>();
            bus.AddSubscription<WorkflowTaskNotification>();

            System.Console.WriteLine("Press enter to start workflow");
            var templatesRequest = new GetWorkflowTemplatesRequest()
            {
                AuditContext = "Console App",
                CorrelationId = Guid.NewGuid(),
                TargetObjectType = "PocketBoss.Samples.User",
                TenantId = System.Guid.Empty
            };


            GetWorkflowTemplatesResponse templateLookupData = null;
            long workflowTemplateId;



            templateLookupData = bus.Send<GetWorkflowTemplatesRequest, GetWorkflowTemplatesResponse>(templatesRequest).Result;

            System.Console.WriteLine("Workflows Registered:" + templateLookupData.WorkflowTemplates.Count());

            if (!templateLookupData.WorkflowTemplates.Exists(t => t.WorkflowTemplateName.Contains("Sample Workflow")))
            {
                System.Console.WriteLine("Test Workflow not seeded");
                System.Console.Read();
                return;
            }
            else
            {
                workflowTemplateId = templateLookupData.WorkflowTemplates[0].WorkflowTemplateId;
            }

            while (Console.ReadLine() != null)
            {
                WorkflowTemplate workflowTemplate = new WorkflowTemplate();
                GetSingleWorkflowTemplateRequest templateRequest = new GetSingleWorkflowTemplateRequest()
                {
                    AuditContext = "Console App",
                    CorrelationId = Guid.NewGuid(),
                    TenantId = System.Guid.Empty,
                    WorkflowTemplateId = workflowTemplateId
                };
                GetSingleWorkflowTemplateResponse searchResponse = bus.Send<GetSingleWorkflowTemplateRequest, GetSingleWorkflowTemplateResponse>(templateRequest).Result;
                workflowTemplate = searchResponse.WorkflowTemplate;


                var startWorkflowInstance = new InitiateWorkflowRequest()
                {
                    AuditContext = "Console App",
                    CorrelationId = Guid.NewGuid(),
                    TargetObjectId = DateTime.Now.Ticks.ToString(),
                    TargetObjectType = workflowTemplate.TargetObjectType,
                    TenantId = System.Guid.Empty,
                    WorkflowTemplateName = workflowTemplate.WorkflowName
                };

                long workflowInstanceId = 0;
                InitiateWorkflowResponse initializeResponse = bus.Send<InitiateWorkflowRequest, InitiateWorkflowResponse>(startWorkflowInstance).Result;
                workflowInstanceId = initializeResponse.WorkflowInstanceId;
                System.Console.WriteLine("Workflow InstanceId = " + workflowInstanceId.ToString());
            }
        }
    }
}
