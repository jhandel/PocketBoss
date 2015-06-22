using Newtonsoft.Json;
using PocketBoss.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBoss.Models
{
    [Serializable]
    public class WorkflowInstance:MultiTenantEntityBase
    {
        public WorkflowInstance()
        {
            States = new List<State>();
            Tasks = new List<Task>();
            Events = new List<WorkflowEvent>();
            WorkflowTemplate = new WorkflowTemplate();
        }
        #region Relationships
        public ICollection<State> States { get; set; }
        public ICollection<Task> Tasks { get; set; }
        public ICollection<WorkflowEvent> GlobalEvents { get; set; }
        public WorkflowTemplate WorkflowTemplate { get; set; }
        public ICollection<WorkflowEvent> Events { get; set; }
        #endregion

        #region Relationship Keys
        public long WorkflowTemplateId { get; set; }
        public Guid WorkflowTemplateTenantId { get; set; }
        #endregion

        #region Properties
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Completed { get; set; }
        public Guid TargetObjectTenantId { get; set; }
        public string TargetObjectId { get; set; }
        public string TargetObjectType { get; set; }
        public string CreatedBy { get; set; }

        [NotMapped]
        [AuditIgnore]
        public State CurrentState
        {
            get
            {
                return States.OrderByDescending(x=>x.Id).FirstOrDefault(x => x.IsCurrent == true);
            }
            set
            {
                States.Where(x => x.IsCurrent == true).ToList().ForEach(x => x.IsCurrent = false);
                value.IsCurrent = true;
                if (value.WorkflowInstance != this)
                {
                    value.WorkflowInstance = this;
                    value.WorkflowInstanceId = this.Id;
                    value.WorkflowInstanceTenantId = this.TenantId;
                }
                if (!States.Contains(value))
                {
                    States.Add(value);
                }
            }
        }
        #endregion
    }
    #region document classes
    [Serializable]
    [AuditIgnore]
    public class WorkflowEvent : MultiTenantEntityBase
    {
        #region Relationships

        [JsonIgnore]
        public WorkflowInstance WorkflowInstance { get; set; }
        public EventType EventType { get; set; }

        [JsonIgnore]
        public WorkflowInstance WorkflowParent { get; set; }

        [JsonIgnore]
        public Task TaskParent { get; set; }

        [JsonIgnore]
        public State StateParent { get; set; }
        #endregion

        #region Relationship Keys
        public long WorkflowInstanceId { get; set; }
        public Guid WorkflowInstanceTenantId { get; set; }
        public long? WorkflowParentId { get; set; }
        public Guid? WorkflowParentTenantId { get; set; }
        public long? TaskParentId { get; set; }
        public Guid? TaskParentTenantId { get; set; }
        public long? StateParentId { get; set; }
        public Guid? StateParentTenantId { get; set; }
        #endregion

        #region Properties
        public DateTime EventDate { get; set; }
        public string EventMetaData { get; set; }
        public string ExecutedBy { get; set; }
        #endregion
    }
    [Serializable]
    [AuditIgnore]
    public class State : MultiTenantEntityBase
    {
        public State()
        {
            Tasks = new List<Task>();
        }
        #region Relationships
        public ICollection<Task> Tasks { get; set; }

        public WorkflowEvent TransitionEvent { get; set; }
        public StateTemplate StateTemplate { get; set; }

        [JsonIgnore]
        public WorkflowInstance WorkflowInstance { get; set; }
        public ICollection<WorkflowEvent> Events { get; set; }
        #endregion

        #region Relationship Keys
        public long? TransitionEventId { get; set; }
        public Guid? TransitionEventTenantId { get; set; }
        public long StateTemplateId { get; set; }
        public Guid StateTemplateTenantId { get; set; }
        public long WorkflowInstanceId { get; set; }
        public Guid WorkflowInstanceTenantId { get; set; }
        #endregion

        #region Properties
        public bool LastState { get; set; }
        public DateTime TransitionDate { get; set; }
        public string TransitionedBy { get; set; }
        public string MetaData { get; set; }
        [NotMapped]
        public List<Task> CurrentTasks
        {
            get
            {
                return Tasks.Where(x => x.IsCurrentTask == true).ToList();
            }
        }
        public bool IsCurrent { get; set; }
        #endregion

    }
    [Serializable]
    [AuditIgnore]
    public class Task : MultiTenantEntityBase
    {

        #region Relationships
        public WorkflowEvent TransitionEvent { get; set; }

        [JsonIgnore]
        public WorkflowInstance WorkflowInstance { get; set; }

        [JsonIgnore]
        public State State { get; set; }
        public TaskTemplate TaskTemplate { get; set; }
        public ICollection<WorkflowEvent> Events { get; set; }
        #endregion

        #region Relationship Keys

        public long? TransitionEventId { get; set; }
        public Guid? TransitionEventTenantId { get; set; }
        public long WorkflowInstanceId { get; set; }
        public Guid WorkflowInstanceTenantId { get; set; }
        public long StateId { get; set; }
        public Guid StateTenantId { get; set; }
        public long TaskTemplateId { get; set; }
        public Guid TaskTemplateTenantId { get; set; }
        #endregion

        #region Properties
        public bool LastTask { get; set; }
        public bool IsCurrentTask { get; set; }
        public DateTime TransitionDate { get; set; }
        public string TransitionedBy { get; set; }
        public string MetaData { get; set; }
        #endregion
    }
    #endregion
}
