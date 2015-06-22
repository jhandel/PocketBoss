
using PocketBoss.Messages;
using PocketBoss.Models;
using PocketBoss.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using PocketBoss.Common.Messaging;
using PocketBoss.Messages.Events;

namespace PocketBoss.Processor
{
    class Utils
    {
        internal static WorkflowTemplate GetWorkflowTemplate(DataContext dataContext, long? id, string objType = null, string workflowName = null)
        {
            var data = dataContext.WorkflowTemplates
                .Where(x => (x.Id == id || id == null) 
                    && (x.TargetObjectType == objType || objType == null) 
                    && (workflowName == null || x.WorkflowName == workflowName)
                    && (id != null || workflowName != null)
                    )
                .Include("EventsTypes")
                .Include("States.Tasks")
                .Include("States")
                .OrderByDescending(x=> x.Id)
                .Select(x => x).FirstOrDefault();
            data.States.AsParallel().ForAll(x =>
            {
                x.WorkflowTemplate = data;
                x.Tasks.AsParallel().ForAll(y =>
                {
                    y.StateTemplate = x;
                    y.WorkflowTemplate = data;
                });
            });
            data.EventsTypes.AsParallel().ForAll(x => x.WorkflowTemplate = data);
            return data;
        }

        internal static WorkflowInstance GetWorkflowInstance(DataContext dataContext, long id, string objId, string objType)
        {
            var data = dataContext.WorkflowInstances
                .Where(x => x.Id == id && x.TargetObjectId == objId && x.TargetObjectType == objType)
                .Include(x => x.Events)
                .Include(x => x.States.Select(y => y.StateTemplate))
                .Include(x => x.States.Select(y => y.TransitionEvent))
                .Include(x => x.States.Select(y => y.Tasks.Select(t => t.TransitionEvent)))
                .Include(x => x.States.Select(y => y.Tasks.Select(t => t.TaskTemplate)))
                .Select(x => x).FirstOrDefault();

            data.WorkflowTemplate = GetWorkflowTemplate(dataContext, data.WorkflowTemplateId);

            return data;
        }

        internal static void PublishNewTasks(IMessagingService bus, WorkflowInstance newWorkflow, string auditContext)
        {
            newWorkflow.CurrentState.CurrentTasks.ForEach(x =>
            {
                bus.Publish<WorkflowTaskNotification>(new WorkflowTaskNotification()
                {
                    AuditContext = auditContext,
                    CorrelationId = Guid.NewGuid(),
                    State = newWorkflow.CurrentState.StateTemplate.StateName,
                    TargetObjectId = newWorkflow.TargetObjectId,
                    TargetObjectType = newWorkflow.TargetObjectType,
                    TenantId = newWorkflow.TenantId,
                    WorkflowInstanceId = newWorkflow.Id,
                    WorkflowTemplateName = newWorkflow.WorkflowTemplate.WorkflowName,
                    StateId = newWorkflow.CurrentState.Id,
                    TaskId = x.Id,
                    Task = x.TaskTemplate.TaskName.Replace(newWorkflow.CurrentState.StateTemplate.StateName + "|",""),
                    IsComplete = x.LastTask,
                });
            });
        }

        internal static void PublishWorkflowState(IMessagingService bus, WorkflowInstance newWorkflow, string auditContext)
        {
            bus.Publish<WorkflowStateNotification>(new WorkflowStateNotification()
            {
                AuditContext = auditContext,
                CorrelationId = Guid.NewGuid(),
                IsComplete = newWorkflow.CurrentState.StateTemplate.LastState,
                State = newWorkflow.CurrentState.StateTemplate.StateName,
                StateId = newWorkflow.CurrentState.Id,
                TargetObjectId = newWorkflow.TargetObjectId,
                TargetObjectType = newWorkflow.TargetObjectType,
                TenantId = newWorkflow.TenantId,
                WorkflowInstanceId = newWorkflow.Id,
                WorkflowTemplateName = newWorkflow.WorkflowTemplate.WorkflowName,
            });
        }
    }
}
