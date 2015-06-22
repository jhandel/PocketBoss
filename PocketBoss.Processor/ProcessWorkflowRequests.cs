using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PocketBoss.Models;
using PocketBoss.Data;
using PocketBoss.Messages;
using System.Data.Entity;
using Microsoft.Practices.Unity;
using PocketBoss.Common;
using PocketBoss.Common.Messaging;
using PocketBoss.Messages.Events;
using PocketBoss.Messages.Commands;
using PocketBoss.Messages.Commands.Responses;


namespace PocketBoss.Processor
{
    public class ProcessWorkflowRequests
    {
        IMessagingService _bus { get; set; }

        public ProcessWorkflowRequests(IMessagingService bus) {
            _bus = bus;

        }

        public class StateLock
        {
            public long WorkflowInstanceId { get; set; }
            public string StateName { get; set; }
            public Guid WorkflowInstanceTenantId { get; set; }
        }
        public static List<StateLock> lockedStates = new List<StateLock>();

        public void GetSingleWorkflowTemplateRequestHandler(GetSingleWorkflowTemplateRequest message)
        {
            GetSingleWorkflowTemplateResponse response = null;
            try
            {
                response = new GetSingleWorkflowTemplateResponse();
                using (var dataContext = new DataContext(message.TenantId, message.AuditContext))
                {
                    long? id = message.WorkflowTemplateId;
                    var name = !string.IsNullOrEmpty(message.WorkflowTemplateName) ? message.WorkflowTemplateName : null;

                    response.AuditContext = message.AuditContext;
                    response.CorrelationId = message.CorrelationId;
                    response.TenantId = message.TenantId;
                    response.WorkflowTemplate = Utils.GetWorkflowTemplate(dataContext, id, null, name);
                }
                _bus.Reply(response);
            }
            catch (Exception exc)
            {
                response = null;
                _bus.Reply(response, 0);
            }
        }
        public void GetWorkflowInstanceDetailsRequestHandler(GetWorkflowInstanceDetailsRequest message)
        {
            GetWorkflowInstanceDetailsResponse response = null;
            try
            {
                response = new GetWorkflowInstanceDetailsResponse();
                using (var dataContext = new DataContext(message.TenantId, message.AuditContext))
                {
                    var id = message.WorkflowInstanceId;
                    var objId = message.TargetObjectId;
                    var objType = message.TargetObjectType;
                    var tenantId = message.TenantId;
                    var data = Utils.GetWorkflowInstance(dataContext, id, objId, objType);
                    response.AuditContext = message.AuditContext;
                    response.CorrelationId = message.CorrelationId;
                    response.TenantId = message.TenantId;
                    response.TargetObjectId = objId;
                    response.TargetObjectType = objType;
                    response.WorkFlowInstance = data;
                    _bus.Reply(response);
                }
            }
            catch (Exception exc)
            {
                response = null;
                _bus.Reply(response, 0);
            }

        }

        public void GetWorkflowTemplatesRequestHandler(GetWorkflowTemplatesRequest message)
        {
            GetWorkflowTemplatesResponse response = null;
            try
            {
                response = new GetWorkflowTemplatesResponse();
                response.AuditContext = message.AuditContext;
                response.TenantId = message.TenantId;
                response.CorrelationId = message.CorrelationId;
                response.WorkflowTemplates = new List<GetWorkflowTemplatesResponse.WorkflowTemplateSummary>();



                using (var dataContext = new DataContext(message.TenantId, message.AuditContext))
                {
                    var data = dataContext.WorkflowTemplates.Where(x => x.TargetObjectType == message.TargetObjectType).Select(x => new GetWorkflowTemplatesResponse.WorkflowTemplateSummary()
                    {
                        OpenWorkflowsRunning = x.WorkflowInstances.Count(c => c.Completed == false),
                        WorkflowsCompleted = x.WorkflowInstances.Count(c => c.Completed == true),
                        WorkflowTemplateDescription = x.WorkflowDescription,
                        WorkflowTemplateId = x.Id,
                        WorkflowTemplateName = x.WorkflowName,
                    }).ToList();
                    response.WorkflowTemplates = data;
                }

                _bus.Reply(response);
            }
            catch (Exception exc)
            {
                response = null;
                _bus.Reply(response, 0);
            }

        }

        public void InitiateWorkflowRequestHandler(InitiateWorkflowRequest message)
        {
            InitiateWorkflowResponse response = null;
            try
            {
                response = new InitiateWorkflowResponse();
                using (var dataContext = new DataContext(message.TenantId, message.AuditContext))
                {
                    response.AuditContext = message.AuditContext;
                    response.CorrelationId = message.CorrelationId;
                    response.TenantId = message.TenantId;

                    var newWorkflow = makeNewWorkflowInstance(message, dataContext, _bus);
                    response.WorkflowInstanceId = newWorkflow.Id;
                    _bus.Reply(response);

                    Utils.PublishWorkflowState(_bus, newWorkflow, message.AuditContext);
                    Utils.PublishNewTasks(_bus, newWorkflow, message.AuditContext);
                }
            }
            catch (Exception exc)
            {
                response = null;
                _bus.Reply(response, 0);
            }
        }

        public void RecordStateActionHandler(RecordStateAction message)
        {
            var workflowId = message.WorkflowInstanceId;
            var tenantId = message.TenantId;
            var auditContext = message.AuditContext;
            var objId = message.TargetObjectId;
            var objType = message.TargetObjectType;
            var stateEvent = message.Event;
            var state = message.State;

            using (var dataContext = new DataContext(message.TenantId, message.AuditContext))
            {
                try
                {
                    var data = Utils.GetWorkflowInstance(dataContext, workflowId, objId, objType);
                    var transition = data.WorkflowTemplate.EventsTypes.FirstOrDefault(x => x.Event == stateEvent && ((x.Type == EventType.EventLevel.GLOBAL) || (x.Type == EventType.EventLevel.STATE && x.ParentName == state)));

                    data = Utils.GetWorkflowInstance(dataContext, workflowId, objId, objType);
                    if (data == null)
                    {
                        return;
                    }
                    var stateData = data.States.Where(x => (x.Id == message.StateId)).FirstOrDefault();
                    if (stateData == null)
                    {
                        stateData = data.States.Where(x => (x.StateTemplate.StateName == message.State)).OrderByDescending(x => x.Id).FirstOrDefault();
                        if (stateData == null)
                        {
                            return;
                        }
                    }

                    WorkflowEvent newEvent = null;

                    if (transition != null)
                    {
                        newEvent = new WorkflowEvent()
                        {
                            EventDate = DateTime.UtcNow,
                            EventMetaData = null,
                            EventType = transition,
                            WorkflowInstance = data,
                            ExecutedBy = message.AuditContext,
                        };
                        if (transition.Type == EventType.EventLevel.GLOBAL)
                        {
                            newEvent.WorkflowParent = data;
                        }
                        else
                        {
                            newEvent.StateParent = stateData;
                        }
                        data.Events.Add(newEvent);
                    }
                    else
                    {
                        return;
                    }
                    if (data.CurrentState.Id == stateData.Id || data.WorkflowTemplate.EventsTypes.Count(x => x.Type == EventType.EventLevel.GLOBAL && x.Event == stateEvent) > 0)
                    {
                        if (transition != null)
                        {
                            var nextState = data.WorkflowTemplate.States.FirstOrDefault(x => x.StateName == transition.MoveTo);
                            if (nextState != null)
                            {


                                var previousState = data.CurrentState;
                                data.CurrentState.IsCurrent = false;
                                data.CurrentState = new State()
                                {
                                    LastState = nextState.LastState,
                                    MetaData = null,
                                    TransitionDate = DateTime.UtcNow,
                                    TransitionedBy = auditContext,
                                    TransitionEvent = newEvent,
                                    Tasks = new List<Models.Task>(),
                                    StateTemplate = nextState,
                                };
                                data.CurrentState.StateTemplate.Tasks.Where(x => x.FirstTask == true).ToList().ForEach(x =>
                                {
                                    data.CurrentState.Tasks.Add(new Models.Task()
                                    {
                                        IsCurrentTask = true,
                                        State = data.CurrentState,
                                        TaskTemplate = x,
                                        TransitionDate = DateTime.UtcNow,
                                        TransitionedBy = message.AuditContext,
                                        WorkflowInstance = data,
                                        LastTask = x.LastTask,
                                    });
                                }
                                );
                            }
                            data.Completed = data.CurrentState.LastState;
                            if (data.Completed)
                            {
                                data.EndDate = DateTime.UtcNow;
                            }
                            dataContext.SaveChanges();
                            Utils.PublishWorkflowState(_bus, data, message.AuditContext);
                            Utils.PublishNewTasks(_bus, data, message.AuditContext);

                        }
                    }
                    else
                    {
                        dataContext.SaveChanges();
                    }
                }
                catch (Exception exc)
                {
                }
            }
        }

        public void RecordTaskActionHandler(RecordTaskAction message)
        {
            
            var workflowId = message.WorkflowInstanceId;
            var tenantId = message.TenantId;
            var auditContext = message.AuditContext;
            var objId = message.TargetObjectId;
            var objType = message.TargetObjectType;
            var taskEvent = message.Event;
            var task = message.State + "|" + message.Task;
            var state = message.State;
            var workflowName = message.WorkflowTemplateName;
            var stateNameForLock = (string.IsNullOrEmpty(state) ? message.StateId.ToString() : state);
            var stateLock = new StateLock()
            {
                WorkflowInstanceId = workflowId,
                WorkflowInstanceTenantId = tenantId,
                StateName = stateNameForLock,
            };
            while (lockedStates.Exists(x => x.WorkflowInstanceId == workflowId && x.StateName == stateNameForLock && x.WorkflowInstanceTenantId == tenantId))
            {
                System.Threading.Thread.Sleep(100);
            }
            lockedStates.Add(stateLock);
            try
            {
                //System.Console.WriteLine("Event Received : " + taskEvent);
                WorkflowInstance data = null;
                using (var dataContext = new DataContext(message.TenantId, auditContext))
                {

                    try
                    {
                        data = Utils.GetWorkflowInstance(dataContext, workflowId, objId, objType);
                        if (data == null)
                        {
                            return;
                        }
                        var stateData = data.States.Where(x => (x.Id == message.StateId)).FirstOrDefault();
                        if (stateData == null)
                        {
                            stateData = data.States.Where(x => (x.StateTemplate.StateName == message.State)).OrderByDescending(x => x.Id).FirstOrDefault();
                            if (stateData == null)
                            {
                                return;
                            }
                        }
                        var taskData = stateData.Tasks.Where(x => (x.Id == message.TaskId)).FirstOrDefault();
                        if (taskData == null)
                        {
                            taskData = stateData.Tasks.Where(x => (x.TaskTemplate.TaskName == task)).OrderByDescending(x => x.Id).FirstOrDefault();
                            if (taskData == null)
                            {
                                return;
                            }
                        }
                        var eventData = data.WorkflowTemplate.EventsTypes.FirstOrDefault(x => x.ParentName == taskData.TaskTemplate.TaskName && x.Event == taskEvent && x.Type == EventType.EventLevel.TASK);
                        if (eventData == null)
                        {
                            return;
                        }
                        var workflowEvent = new WorkflowEvent()
                        {
                            EventDate = DateTime.UtcNow,
                            EventMetaData = null,
                            EventType = eventData,
                            ExecutedBy = auditContext,
                            TaskParent = taskData,
                            WorkflowInstance = data,
                        };
                        data.Events.Add(workflowEvent);


                        if (data.CurrentState.Id == stateData.Id && taskData.IsCurrentTask == true)
                        {
                            TaskTemplate nextTaskTemplate = stateData.StateTemplate.Tasks.FirstOrDefault(x => x.TaskName == eventData.MoveTo);
                            if (nextTaskTemplate == null)
                            {
                                return;
                            }
                            taskData.IsCurrentTask = false;
                            PocketBoss.Models.Task newTask = new Models.Task()
                            {
                                TaskTemplate = nextTaskTemplate,
                                IsCurrentTask = true,
                                LastTask = nextTaskTemplate.LastTask,
                                State = stateData,
                                WorkflowInstance = data,
                                TransitionDate = DateTime.UtcNow,
                                TransitionedBy = auditContext,
                                TransitionEvent = workflowEvent,
                            };
                            stateData.Tasks.Add(newTask);
                            dataContext.SaveChanges();
                            publishNewTasks(data, newTask, message.AuditContext, message.TenantId, _bus);
                        }
                    }
                    catch (Exception exc)
                    {
                    }

                }
                using (var dataContext = new DataContext(message.TenantId, auditContext))
                {
                    if (dataContext.Tasks.Count(x => x.LastTask == false && x.IsCurrentTask == true && x.State.IsCurrent == true && x.WorkflowInstanceId == data.Id) == 0)
                    {
                        updateStateAsComplete(data, message.AuditContext, message.TenantId, _bus);
                    }
                }
            }
            catch (Exception exc)
            {
            }
            finally
            {
                lockedStates.Remove(stateLock);
            }

        }

        private void updateStateAsComplete(WorkflowInstance wf, string auditContext, Guid tenantId, IMessagingService bus)
        {
            var message = new RecordStateAction()
            {
                AuditContext = auditContext,
                CorrelationId = Guid.NewGuid(),
                State = wf.CurrentState.StateTemplate.StateName,
                StateId = wf.CurrentState.Id,
                TargetObjectId = wf.TargetObjectId,
                TargetObjectType = wf.TargetObjectType,
                TenantId = tenantId,
                WorkflowInstanceId = wf.Id,
                WorkflowTemplateName = wf.WorkflowTemplate.WorkflowName,
                Event = "All_Tasks_Completed"
            };
            RecordStateActionHandler(message);

            
        }

        private static void publishNewTasks(WorkflowInstance wf, PocketBoss.Models.Task task, string auditContext, Guid tenantId, IMessagingService bus)
        {

            bus.Publish<WorkflowTaskNotification>(new WorkflowTaskNotification()
            {
                AuditContext = auditContext,
                CorrelationId = Guid.NewGuid(),
                State = wf.CurrentState.StateTemplate.StateName,
                TargetObjectId = wf.TargetObjectId,
                TargetObjectType = wf.TargetObjectType,
                TenantId = tenantId,
                WorkflowInstanceId = wf.Id,
                WorkflowTemplateName = wf.WorkflowTemplate.WorkflowName,
                StateId = wf.CurrentState.Id,
                Task = task.TaskTemplate.TaskName.Replace(wf.CurrentState.StateTemplate.StateName + "|", ""),
                TaskId = task.Id,
                IsComplete = task.LastTask,
            });
        }

        private static WorkflowInstance makeNewWorkflowInstance(InitiateWorkflowRequest message, DataContext dataContext, IMessagingService bus)
        {
            var objType = message.TargetObjectType;
            var newWorkflow = new WorkflowInstance();
            newWorkflow.TargetObjectId = message.TargetObjectId;
            newWorkflow.TargetObjectTenantId = message.TenantId;
            newWorkflow.WorkflowTemplate = Utils.GetWorkflowTemplate(dataContext, null, objType, message.WorkflowTemplateName);
            newWorkflow.StartDate = DateTime.UtcNow;
            newWorkflow.Completed = false;
            newWorkflow.CurrentState = new State()
            {
                LastState = false,
                MetaData = null,
                StateTemplate = newWorkflow.WorkflowTemplate.States.FirstOrDefault(x => x.FirstState),
                TransitionDate = DateTime.UtcNow,
                TransitionedBy = message.AuditContext,
                TransitionEvent = new WorkflowEvent()
                {
                    EventDate = DateTime.UtcNow,
                    EventType = newWorkflow.WorkflowTemplate.EventsTypes.FirstOrDefault(x => x.Event == "Start_Workflow"),
                    ExecutedBy = message.AuditContext,
                    StateParent = newWorkflow.CurrentState,
                    WorkflowInstance = newWorkflow,
                },
                Tasks = new List<Models.Task>(),
            };
            newWorkflow.CurrentState.StateTemplate.Tasks.Where(x => x.FirstTask == true).ToList().ForEach(x =>
            {
                newWorkflow.CurrentState.Tasks.Add(new Models.Task()
                {
                    IsCurrentTask = true,
                    State = newWorkflow.CurrentState,
                    TaskTemplate = x,
                    TransitionDate = DateTime.UtcNow,
                    TransitionedBy = message.AuditContext,
                    WorkflowInstance = newWorkflow,
                });
            }
            );
            newWorkflow.TargetObjectType = message.TargetObjectType;
            dataContext.WorkflowInstances.Add(newWorkflow);
            dataContext.SaveChanges();
            return newWorkflow;
        }

    }
}
