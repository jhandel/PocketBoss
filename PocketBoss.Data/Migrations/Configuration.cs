namespace PocketBoss.Data.Migrations
{
    using PocketBoss.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PocketBoss.Data.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PocketBoss.Data.DataContext context)
        {
            using (var db = new DataContext(System.Guid.Empty, "Seeding Method"))
            {
                string humanSeedId = "Version 1.1";//Change me to make a new workflow
                var currentOrderWorkflow = db.WorkflowTemplates.OrderByDescending(x=> x.Id).ToList().FirstOrDefault();
                if (currentOrderWorkflow!= null && currentOrderWorkflow.HumanSeedId == humanSeedId)
                {
                    return;
                }
                var template = new WorkflowTemplate();
                template.HumanSeedId = humanSeedId;
                template.WorkflowDescription = "A sample workflow";
                template.TargetObjectType = "PocketBoss.Samples.User";
                template.WorkflowName = "Sample Workflow";
                template.EventsTypes = new System.Collections.Generic.List<EventType>();
                template.States = new System.Collections.Generic.List<StateTemplate>();


                #region Adding Global EventTypes
                addEventType("Start_Workflow", "CheckForUser", "GLOBAL", EventType.EventLevel.GLOBAL, template);
                addEventType("Restart_Workflow", "CheckForUser", "GLOBAL", EventType.EventLevel.GLOBAL, template);
                addEventType("Cancel_Workflow", "Cancelled", "GLOBAL", EventType.EventLevel.GLOBAL, template);
                #endregion
                #region Add States
                {
                    #region Hold States
                    {
                        #region Holding States
                        {
                            var newState = makeStateTemplate("UserCheckHold", false, false, template);
                            //Add StateEvents
                            addEventType("Release", "CheckForUser", newState.StateName, EventType.EventLevel.STATE, template);
                        }
                        #endregion
                    }
                    #endregion
                    #region Functional Workflow
                    {
                        #region CheckForUser
                        {
                            var newState = makeStateTemplate("CheckForUser", true, false, template);
                            //Add StateEvents
                            addEventType("All_Tasks_Completed", "UserFound", newState.StateName, EventType.EventLevel.STATE, template);
                            addEventType("User_Check_Failed", "UserCheckHold", newState.StateName, EventType.EventLevel.STATE, template);
                            //Add Tasks
                            newState.Tasks = new List<TaskTemplate>();
                            addSimpleTask(template, newState, "CanAccessAd", true, false, "AD_Online", "CheckUserName");
                            addSimpleTask(template, newState, "CheckUserName", false, false, "User_Found", "CompletedCheckForUser");
                            addSimpleTask(template, newState, "CompletedCheckForUser", false, true);
                        }
                        #endregion
                    }
                    #endregion
                    #region Add End States
                    {
                        #region User Found State
                        {
                            var newState = makeStateTemplate("UserFound", false, true, template);
                            //Add StateEvents

                            //Add Tasks
                        }
                        #endregion
                        #region Cancelled
                        {
                            var newState = makeStateTemplate("Cancelled", false, true, template);
                            //Add StateEvents

                            //Add Tasks
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion
                db.Entry<WorkflowTemplate>(template).State = EntityState.Added;
                db.SaveChanges();

            }
        }
        private static StateTemplate makeStateTemplate(string StateName, bool isFirst, bool isLast, WorkflowTemplate template)
        {
            var newState = new StateTemplate();
            newState.StateName = StateName;
            newState.FirstState = isFirst;
            newState.LastState = isLast;
            newState.WorkflowTemplate = template;
            template.States.Add(newState);
            return newState;
        }
        private static void addEventType(string eventName, string goTo, string parentName, EventType.EventLevel level, WorkflowTemplate template)
        {
            template.EventsTypes.Add(new EventType()
            {
                Event = eventName,
                MoveTo = goTo,
                ParentName = parentName,
                TenantId = template.TenantId,
                Type = level,
                WorkflowTemplate = template,
            });
        }
        private static void addSimpleTask(WorkflowTemplate template, StateTemplate newState, string taskName, bool isFirst = false, bool isLast = false, string successEventName = null, string successNextTask = null)
        {
            var newTask = new TaskTemplate();
            newTask.TaskName = newState.StateName + "|" + taskName;
            newTask.StateTemplate = newState;
            newTask.FirstTask = isFirst;
            newTask.LastTask = isLast;
            newTask.WorkflowTemplate = template;
            newState.Tasks.Add(newTask);

            //Add Task Events
            if (!string.IsNullOrEmpty(successNextTask) && !string.IsNullOrEmpty(successEventName))
            {
                addEventType(successEventName, newState.StateName + "|" + successNextTask, newTask.TaskName, EventType.EventLevel.TASK, template);
            }
        }
    }
}
