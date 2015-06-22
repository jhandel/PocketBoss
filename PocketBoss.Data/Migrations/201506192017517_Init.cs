namespace PocketBoss.Data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.workflow_AuditLog",
                c => new
                    {
                        TenantId = c.Guid(nullable: false),
                        Id = c.Long(nullable: false, identity: true),
                        Discriminator = c.String(nullable: false),
                        DiscriminatorId = c.Long(nullable: false),
                        DiscriminatorTenantId = c.Guid(nullable: false),
                        Field = c.String(),
                        Action = c.String(),
                        FromValue = c.String(),
                        ToValue = c.String(),
                        ChangedOn = c.DateTime(nullable: false),
                        ChangedBy = c.String(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_AuditLog_MultiTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => new { t.TenantId, t.Id });
            
            CreateTable(
                "dbo.workflow_EventType",
                c => new
                    {
                        TenantId = c.Guid(nullable: false),
                        Id = c.Long(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        ModifiedOn = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        WorkflowTemplateId = c.Long(nullable: false),
                        WorkflowTemplateTenantId = c.Guid(nullable: false),
                        Event = c.String(maxLength: 100),
                        MoveTo = c.String(),
                        Type = c.Int(nullable: false),
                        ParentName = c.String(maxLength: 100),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_EventType_MultiTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => new { t.TenantId, t.Id })
                .ForeignKey("dbo.workflow_WorkflowTemplate", t => new { t.WorkflowTemplateTenantId, t.WorkflowTemplateId }, cascadeDelete: true)
                .Index(t => new { t.WorkflowTemplateTenantId, t.WorkflowTemplateId })
                .Index(t => t.Event)
                .Index(t => t.ParentName);
            
            CreateTable(
                "dbo.workflow_WorkflowTemplate",
                c => new
                    {
                        TenantId = c.Guid(nullable: false),
                        Id = c.Long(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        ModifiedOn = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        WorkflowName = c.String(),
                        WorkflowDescription = c.String(),
                        TargetObjectType = c.String(),
                        HumanSeedId = c.String(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_WorkflowTemplate_MultiTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => new { t.TenantId, t.Id });
            
            CreateTable(
                "dbo.workflow_StateTemplate",
                c => new
                    {
                        TenantId = c.Guid(nullable: false),
                        Id = c.Long(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        ModifiedOn = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        WorkflowTemplateId = c.Long(nullable: false),
                        WorkflowTemplateTenantId = c.Guid(nullable: false),
                        StateName = c.String(),
                        LastState = c.Boolean(nullable: false),
                        FirstState = c.Boolean(nullable: false),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_StateTemplate_MultiTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => new { t.TenantId, t.Id })
                .ForeignKey("dbo.workflow_WorkflowTemplate", t => new { t.WorkflowTemplateTenantId, t.WorkflowTemplateId }, cascadeDelete: true)
                .Index(t => new { t.WorkflowTemplateTenantId, t.WorkflowTemplateId });
            
            CreateTable(
                "dbo.workflow_TaskTemplate",
                c => new
                    {
                        TenantId = c.Guid(nullable: false),
                        Id = c.Long(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        ModifiedOn = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        WorkflowTemplateId = c.Long(nullable: false),
                        WorkflowTemplateTenantId = c.Guid(nullable: false),
                        StateTemplateId = c.Long(nullable: false),
                        StateTemplateTenantId = c.Guid(nullable: false),
                        LastTask = c.Boolean(nullable: false),
                        FirstTask = c.Boolean(nullable: false),
                        TaskName = c.String(maxLength: 100),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_TaskTemplate_MultiTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => new { t.TenantId, t.Id })
                .ForeignKey("dbo.workflow_StateTemplate", t => new { t.StateTemplateTenantId, t.StateTemplateId }, cascadeDelete: true)
                .ForeignKey("dbo.workflow_WorkflowTemplate", t => new { t.WorkflowTemplateTenantId, t.WorkflowTemplateId }, cascadeDelete: false)
                .Index(t => new { t.WorkflowTemplateTenantId, t.WorkflowTemplateId })
                .Index(t => new { t.StateTemplateTenantId, t.StateTemplateId })
                .Index(t => t.TaskName);
            
            CreateTable(
                "dbo.workflow_WorkflowInstance",
                c => new
                    {
                        TenantId = c.Guid(nullable: false),
                        Id = c.Long(nullable: false, identity: true),
                        WorkflowTemplateId = c.Long(nullable: false),
                        WorkflowTemplateTenantId = c.Guid(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        Completed = c.Boolean(nullable: false),
                        TargetObjectTenantId = c.Guid(nullable: false),
                        TargetObjectId = c.Long(nullable: false),
                        TargetObjectType = c.String(),
                        CreatedBy = c.String(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_WorkflowInstance_MultiTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => new { t.TenantId, t.Id })
                .ForeignKey("dbo.workflow_WorkflowTemplate", t => new { t.WorkflowTemplateTenantId, t.WorkflowTemplateId }, cascadeDelete: true)
                .Index(t => new { t.WorkflowTemplateTenantId, t.WorkflowTemplateId });
            
            CreateTable(
                "dbo.workflow_WorkflowEvent",
                c => new
                    {
                        TenantId = c.Guid(nullable: false),
                        Id = c.Long(nullable: false, identity: true),
                        WorkflowInstanceId = c.Long(nullable: false),
                        WorkflowInstanceTenantId = c.Guid(nullable: false),
                        WorkflowParentId = c.Long(),
                        WorkflowParentTenantId = c.Guid(),
                        TaskParentId = c.Long(),
                        TaskParentTenantId = c.Guid(),
                        StateParentId = c.Long(),
                        StateParentTenantId = c.Guid(),
                        EventDate = c.DateTime(nullable: false),
                        EventMetaData = c.String(),
                        ExecutedBy = c.String(),
                        EventType_TenantId = c.Guid(),
                        EventType_Id = c.Long(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_WorkflowEvent_MultiTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => new { t.TenantId, t.Id })
                .ForeignKey("dbo.workflow_EventType", t => new { t.EventType_TenantId, t.EventType_Id })
                .ForeignKey("dbo.workflow_State", t => new { t.StateParentTenantId, t.StateParentId })
                .ForeignKey("dbo.workflow_Task", t => new { t.TaskParentTenantId, t.TaskParentId })
                .ForeignKey("dbo.workflow_WorkflowInstance", t => new { t.WorkflowInstanceTenantId, t.WorkflowInstanceId }, cascadeDelete: true)
                .ForeignKey("dbo.workflow_WorkflowInstance", t => new { t.WorkflowParentTenantId, t.WorkflowParentId })
                .Index(t => new { t.WorkflowInstanceTenantId, t.WorkflowInstanceId })
                .Index(t => new { t.WorkflowParentTenantId, t.WorkflowParentId })
                .Index(t => new { t.TaskParentTenantId, t.TaskParentId })
                .Index(t => new { t.StateParentTenantId, t.StateParentId })
                .Index(t => new { t.EventType_TenantId, t.EventType_Id });
            
            CreateTable(
                "dbo.workflow_State",
                c => new
                    {
                        TenantId = c.Guid(nullable: false),
                        Id = c.Long(nullable: false, identity: true),
                        TransitionEventId = c.Long(),
                        TransitionEventTenantId = c.Guid(),
                        StateTemplateId = c.Long(nullable: false),
                        StateTemplateTenantId = c.Guid(nullable: false),
                        WorkflowInstanceId = c.Long(nullable: false),
                        WorkflowInstanceTenantId = c.Guid(nullable: false),
                        LastState = c.Boolean(nullable: false),
                        TransitionDate = c.DateTime(nullable: false),
                        TransitionedBy = c.String(),
                        MetaData = c.String(),
                        IsCurrent = c.Boolean(nullable: false),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_State_MultiTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => new { t.TenantId, t.Id })
                .ForeignKey("dbo.workflow_StateTemplate", t => new { t.StateTemplateTenantId, t.StateTemplateId }, cascadeDelete: false)
                .ForeignKey("dbo.workflow_WorkflowEvent", t => new { t.TransitionEventTenantId, t.TransitionEventId })
                .ForeignKey("dbo.workflow_WorkflowInstance", t => new { t.WorkflowInstanceTenantId, t.WorkflowInstanceId }, cascadeDelete: true)
                .Index(t => new { t.TransitionEventTenantId, t.TransitionEventId })
                .Index(t => new { t.StateTemplateTenantId, t.StateTemplateId })
                .Index(t => new { t.WorkflowInstanceTenantId, t.WorkflowInstanceId });
            
            CreateTable(
                "dbo.workflow_Task",
                c => new
                    {
                        TenantId = c.Guid(nullable: false),
                        Id = c.Long(nullable: false, identity: true),
                        TransitionEventId = c.Long(),
                        TransitionEventTenantId = c.Guid(),
                        WorkflowInstanceId = c.Long(nullable: false),
                        WorkflowInstanceTenantId = c.Guid(nullable: false),
                        StateId = c.Long(nullable: false),
                        StateTenantId = c.Guid(nullable: false),
                        TaskTemplateId = c.Long(nullable: false),
                        TaskTemplateTenantId = c.Guid(nullable: false),
                        LastTask = c.Boolean(nullable: false),
                        IsCurrentTask = c.Boolean(nullable: false),
                        TransitionDate = c.DateTime(nullable: false),
                        TransitionedBy = c.String(),
                        MetaData = c.String(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Task_MultiTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => new { t.TenantId, t.Id })
                .ForeignKey("dbo.workflow_State", t => new { t.StateTenantId, t.StateId }, cascadeDelete: true)
                .ForeignKey("dbo.workflow_TaskTemplate", t => new { t.TaskTemplateTenantId, t.TaskTemplateId }, cascadeDelete: false)
                .ForeignKey("dbo.workflow_WorkflowEvent", t => new { t.TransitionEventTenantId, t.TransitionEventId })
                .ForeignKey("dbo.workflow_WorkflowInstance", t => new { t.WorkflowInstanceTenantId, t.WorkflowInstanceId }, cascadeDelete: false)
                .Index(t => new { t.TransitionEventTenantId, t.TransitionEventId })
                .Index(t => new { t.WorkflowInstanceTenantId, t.WorkflowInstanceId })
                .Index(t => new { t.StateTenantId, t.StateId })
                .Index(t => new { t.TaskTemplateTenantId, t.TaskTemplateId });
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.workflow_WorkflowInstance", new[] { "WorkflowTemplateTenantId", "WorkflowTemplateId" }, "dbo.workflow_WorkflowTemplate");
            DropForeignKey("dbo.workflow_WorkflowEvent", new[] { "WorkflowParentTenantId", "WorkflowParentId" }, "dbo.workflow_WorkflowInstance");
            DropForeignKey("dbo.workflow_WorkflowEvent", new[] { "WorkflowInstanceTenantId", "WorkflowInstanceId" }, "dbo.workflow_WorkflowInstance");
            DropForeignKey("dbo.workflow_WorkflowEvent", new[] { "TaskParentTenantId", "TaskParentId" }, "dbo.workflow_Task");
            DropForeignKey("dbo.workflow_WorkflowEvent", new[] { "StateParentTenantId", "StateParentId" }, "dbo.workflow_State");
            DropForeignKey("dbo.workflow_State", new[] { "WorkflowInstanceTenantId", "WorkflowInstanceId" }, "dbo.workflow_WorkflowInstance");
            DropForeignKey("dbo.workflow_State", new[] { "TransitionEventTenantId", "TransitionEventId" }, "dbo.workflow_WorkflowEvent");
            DropForeignKey("dbo.workflow_Task", new[] { "WorkflowInstanceTenantId", "WorkflowInstanceId" }, "dbo.workflow_WorkflowInstance");
            DropForeignKey("dbo.workflow_Task", new[] { "TransitionEventTenantId", "TransitionEventId" }, "dbo.workflow_WorkflowEvent");
            DropForeignKey("dbo.workflow_Task", new[] { "TaskTemplateTenantId", "TaskTemplateId" }, "dbo.workflow_TaskTemplate");
            DropForeignKey("dbo.workflow_Task", new[] { "StateTenantId", "StateId" }, "dbo.workflow_State");
            DropForeignKey("dbo.workflow_State", new[] { "StateTemplateTenantId", "StateTemplateId" }, "dbo.workflow_StateTemplate");
            DropForeignKey("dbo.workflow_WorkflowEvent", new[] { "EventType_TenantId", "EventType_Id" }, "dbo.workflow_EventType");
            DropForeignKey("dbo.workflow_StateTemplate", new[] { "WorkflowTemplateTenantId", "WorkflowTemplateId" }, "dbo.workflow_WorkflowTemplate");
            DropForeignKey("dbo.workflow_TaskTemplate", new[] { "WorkflowTemplateTenantId", "WorkflowTemplateId" }, "dbo.workflow_WorkflowTemplate");
            DropForeignKey("dbo.workflow_TaskTemplate", new[] { "StateTemplateTenantId", "StateTemplateId" }, "dbo.workflow_StateTemplate");
            DropForeignKey("dbo.workflow_EventType", new[] { "WorkflowTemplateTenantId", "WorkflowTemplateId" }, "dbo.workflow_WorkflowTemplate");
            DropIndex("dbo.workflow_Task", new[] { "TaskTemplateTenantId", "TaskTemplateId" });
            DropIndex("dbo.workflow_Task", new[] { "StateTenantId", "StateId" });
            DropIndex("dbo.workflow_Task", new[] { "WorkflowInstanceTenantId", "WorkflowInstanceId" });
            DropIndex("dbo.workflow_Task", new[] { "TransitionEventTenantId", "TransitionEventId" });
            DropIndex("dbo.workflow_State", new[] { "WorkflowInstanceTenantId", "WorkflowInstanceId" });
            DropIndex("dbo.workflow_State", new[] { "StateTemplateTenantId", "StateTemplateId" });
            DropIndex("dbo.workflow_State", new[] { "TransitionEventTenantId", "TransitionEventId" });
            DropIndex("dbo.workflow_WorkflowEvent", new[] { "EventType_TenantId", "EventType_Id" });
            DropIndex("dbo.workflow_WorkflowEvent", new[] { "StateParentTenantId", "StateParentId" });
            DropIndex("dbo.workflow_WorkflowEvent", new[] { "TaskParentTenantId", "TaskParentId" });
            DropIndex("dbo.workflow_WorkflowEvent", new[] { "WorkflowParentTenantId", "WorkflowParentId" });
            DropIndex("dbo.workflow_WorkflowEvent", new[] { "WorkflowInstanceTenantId", "WorkflowInstanceId" });
            DropIndex("dbo.workflow_WorkflowInstance", new[] { "WorkflowTemplateTenantId", "WorkflowTemplateId" });
            DropIndex("dbo.workflow_TaskTemplate", new[] { "TaskName" });
            DropIndex("dbo.workflow_TaskTemplate", new[] { "StateTemplateTenantId", "StateTemplateId" });
            DropIndex("dbo.workflow_TaskTemplate", new[] { "WorkflowTemplateTenantId", "WorkflowTemplateId" });
            DropIndex("dbo.workflow_StateTemplate", new[] { "WorkflowTemplateTenantId", "WorkflowTemplateId" });
            DropIndex("dbo.workflow_EventType", new[] { "ParentName" });
            DropIndex("dbo.workflow_EventType", new[] { "Event" });
            DropIndex("dbo.workflow_EventType", new[] { "WorkflowTemplateTenantId", "WorkflowTemplateId" });
            DropTable("dbo.workflow_Task",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Task_MultiTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.workflow_State",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_State_MultiTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.workflow_WorkflowEvent",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_WorkflowEvent_MultiTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.workflow_WorkflowInstance",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_WorkflowInstance_MultiTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.workflow_TaskTemplate",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_TaskTemplate_MultiTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.workflow_StateTemplate",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_StateTemplate_MultiTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.workflow_WorkflowTemplate",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_WorkflowTemplate_MultiTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.workflow_EventType",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_EventType_MultiTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("dbo.workflow_AuditLog",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_AuditLog_MultiTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
