
using PocketBoss.Common.Data;
using PocketBoss.Data.Migrations;
using PocketBoss.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBoss.Data
{
    internal class DataContextMigration : MigrateDatabaseToLatestVersion<DataContext, Configuration>
    {
        //public override void InitializeDatabase(DataContext context)
        //{
        //    base.InitializeDatabase(context);
        //}
    }
    public class DataContext : MultiTenantDbContextBase
    {
        static DataContext()
        {
            Database.SetInitializer<DataContext>(new DataContextMigration());
        }
       public DataContext()
            : base("DefaultConnection", System.Guid.Empty, "not a context")
        {
            this.Configuration.LazyLoadingEnabled = false; 
        }
       public DataContext(string nameOrConnectionString, Guid tenantId, string currentUser)
            : base(nameOrConnectionString, tenantId, currentUser)
        {
            this.Configuration.LazyLoadingEnabled = false; 
        }
        public DataContext(Guid tenantId, string currentUser)
            : base("DefaultConnection", tenantId, currentUser)
        {
            this.Configuration.LazyLoadingEnabled = false; 
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Types().Configure(entity => entity.ToTable("workflow_" + entity.ClrType.Name));
            modelBuilder.Entity<WorkflowEvent>().HasOptional<PocketBoss.Models.Task>(x => x.TaskParent).WithMany(x => x.Events).HasForeignKey(x => new { x.TaskParentTenantId, x.TaskParentId });
            modelBuilder.Entity<WorkflowEvent>().HasOptional<PocketBoss.Models.State>(x => x.StateParent).WithMany(x => x.Events).HasForeignKey(x => new { x.StateParentTenantId, x.StateParentId });
            modelBuilder.Entity<WorkflowEvent>().HasOptional<PocketBoss.Models.WorkflowInstance>(x => x.WorkflowParent).WithMany(x => x.GlobalEvents).HasForeignKey(x => new { x.WorkflowParentTenantId, x.WorkflowParentId });
            base.OnModelCreating(modelBuilder);

        }

        public IDbSet<WorkflowTemplate> WorkflowTemplates { get; set; }
        public IDbSet<StateTemplate> StateTemplates { get; set; }
        public IDbSet<TaskTemplate> TaskTemplates { get; set; }
        public IDbSet<EventType> EventTypes { get; set; }

        public IDbSet<WorkflowInstance> WorkflowInstances { get; set; }
        public IDbSet<WorkflowEvent> StateEvents { get; set; }
        public IDbSet<State> States{ get; set; }
        public IDbSet<PocketBoss.Models.Task> Tasks { get; set; }

        public IQueryable<WorkflowTemplate> EagerLoadedWorkflowTemplate
        {
            get
            {
                return this.WorkflowTemplates;
                        
            }
        }
    }
}
