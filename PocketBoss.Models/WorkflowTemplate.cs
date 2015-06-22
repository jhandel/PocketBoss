using Newtonsoft.Json;
using PocketBoss.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace PocketBoss.Models
{
    [Serializable]
    public class WorkflowTemplate : MultiTenantEntityBase, IAuditable
    {

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public WorkflowTemplate()
        {
            States = new List<StateTemplate>();
            EventsTypes = new List<EventType>();
        }
        #region Relationships
        [AuditIgnore]
        public ICollection<StateTemplate> States { get; set; }
        [AuditIgnore]
        public ICollection<EventType> EventsTypes { get; set; }
        [AuditIgnore]

        [JsonIgnore]
        public List<WorkflowInstance> WorkflowInstances { get; set; }
        #endregion

        #region Relationship Keys
       
        #endregion

        #region Properties
        public string WorkflowName { get; set; }
        public string WorkflowDescription { get; set; }
        public string TargetObjectType { get; set; }
        public string HumanSeedId { get; set; }
        #endregion


    }
    #region document classes
    [Serializable]
    public class StateTemplate : MultiTenantEntityBase, IAuditable
    {

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public StateTemplate()
        {
            Tasks = new List<TaskTemplate>();
        }
        #region Relationships

        [AuditIgnore]
        [JsonIgnore]
        public WorkflowTemplate WorkflowTemplate { get; set; }
        [AuditIgnore]
        public ICollection<TaskTemplate> Tasks { get; set; }
        #endregion

        #region Relationship Keys
        public long WorkflowTemplateId { get; set; }
        public Guid WorkflowTemplateTenantId { get; set; }
        #endregion

        #region Properties
        public string StateName { get; set; }
        public bool LastState { get; set; }
        public bool FirstState { get; set; }
        [NotMapped]
        [AuditIgnore]
        public List<TaskTemplate> InitialTasks
        {
            get
            {
                return Tasks.Where(x => x.FirstTask == true).ToList();
            }
        }
        [NotMapped]
        [AuditIgnore]
        [JsonIgnore]
        public List<EventType> EventsTypes
        {
            get
            {
                return WorkflowTemplate.EventsTypes.Where(x => x.Type == EventType.EventLevel.STATE && x.ParentName == StateName).ToList();
            }
        }

        #endregion

    }
    [Serializable]
    public class TaskTemplate : MultiTenantEntityBase, IAuditable
    {

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        #region Relationships
        [NotMapped]
        [AuditIgnore]
        [JsonIgnore]
        public List<EventType> EventsTypes
        {
            get
            {
                return WorkflowTemplate.EventsTypes.Where(x => x.Type == EventType.EventLevel.TASK && x.ParentName == StateTemplate.StateName + "|" + TaskName).ToList();
            }
        }

        [AuditIgnore]
        [JsonIgnore]
        public StateTemplate StateTemplate { get; set; }

        [AuditIgnore]
        [JsonIgnore]
        public WorkflowTemplate WorkflowTemplate { get; set; } 

        #endregion

        #region Relationship Keys
        public long WorkflowTemplateId { get; set; }
        public Guid WorkflowTemplateTenantId { get; set; }
        public long StateTemplateId { get; set; }
        public Guid StateTemplateTenantId { get; set; }
        #endregion

        #region Properties
        public bool LastTask { get; set; }
        public bool FirstTask { get; set; }
        [Index]
        [MaxLength(100)]
        public string TaskName { get; set; }
        #endregion
    }
    [Serializable]
    public class EventType : MultiTenantEntityBase, IAuditable
    {

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public enum EventLevel
        {
            GLOBAL,
            STATE,
            TASK
        }
        #region Relationships
        [AuditIgnore]
        [JsonIgnore]
        public WorkflowTemplate WorkflowTemplate { get; set; }
        #endregion

        #region Relationship Keys
        public long WorkflowTemplateId { get; set; }
        public Guid WorkflowTemplateTenantId { get; set; }
        #endregion

        #region Properties
        [Index]
        [MaxLength(100)]
        public string Event { get; set; }
        public string MoveTo { get; set; }
        public EventLevel Type { get; set; }
        [Index]
        [MaxLength(100)]
        public string ParentName { get; set; }
        #endregion
    }
    #endregion
}
