namespace PocketBoss.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TargetObjectTypeToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.workflow_WorkflowInstance", "TargetObjectId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.workflow_WorkflowInstance", "TargetObjectId", c => c.Long(nullable: false));
        }
    }
}
