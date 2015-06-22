using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFramework.DynamicFilters;

namespace PocketBoss.Common.Data
{
    public abstract class MultiTenantDbContextBase : DbContext, IDisposable
    {
        public Guid TenantId { get; private set; }
        public string CurrentUser { get; private set; }
        public DbSet<AuditLog> AuditLog { get; set; }
        private Dictionary<Type, List<string>> AuditInfo { get; set; }

        public MultiTenantDbContextBase(DbConnection existingConnection, bool contextOwnsConnection, Guid tenantId, string currentUser)
            : base(existingConnection, contextOwnsConnection)
        {
            TenantId = tenantId;
            CurrentUser = currentUser;
        }
        public MultiTenantDbContextBase(ObjectContext objectContext, bool dbContextOwnsObjectContext, Guid tenantId, string currentUser)
            : base(objectContext, dbContextOwnsObjectContext)
        {
            TenantId = tenantId;
            CurrentUser = currentUser;
        }
        public MultiTenantDbContextBase(string nameOrConnectionString, DbCompiledModel model, Guid tenantId, string currentUser)
            : base(nameOrConnectionString, model)
        {
            TenantId = tenantId;
            CurrentUser = currentUser;
        }
        public MultiTenantDbContextBase(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection, Guid tenantId, string currentUser)
            : base(existingConnection, model, contextOwnsConnection)
        {
            TenantId = tenantId;
            CurrentUser = currentUser;
        }
        public MultiTenantDbContextBase(string nameOrConnectionString, Guid tenantId, string currentUser)
            : base(nameOrConnectionString)
        {
            TenantId = tenantId;
            CurrentUser = currentUser;
        }
        public MultiTenantDbContextBase(Guid tenantId, string currentUser)
            : base("DefaultConnection")
        {
            TenantId = tenantId;
            CurrentUser = currentUser;
        }
        public override int SaveChanges()
        {
            setTenantId();
            calculateAuditing();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken)
        {
            setTenantId();
            calculateAuditing();
            return base.SaveChangesAsync(cancellationToken);
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Filter("MultiTenant", (IMultiTenantEntity d, Guid tenantId) => d.TenantId == tenantId, ()=> this.TenantId);
        }
        private void setTenantId()
        {
            foreach (var entry in ChangeTracker.Entries<IMultiTenantEntity>())
            {
                entry.Entity.TenantId = TenantId;
            }
        }

        private void calculateAuditing()
        {
            DateTime now = DateTime.UtcNow;

            //get a list of the changes that the user made
            var logs = new List<AuditLogEntity>();
            foreach (var entry in ChangeTracker.Entries<IAuditable>())
            {
                var entryType = entry.Entity.GetType();
                if (entry.State == EntityState.Added)
                {
                    logs.AddRange(FieldsToLog(entryType)
                        .Select(propertyName => new AuditLogEntity()
                        {
                            Entity = entry.Entity,
                            Log = new AuditLog()
                                        {
                                            Discriminator = entryType.ToString(),
                                            Action = entry.State.ToString(),
                                            Field = propertyName,
                                            FromValue = string.Empty,
                                            ToValue = entry.CurrentValues[propertyName] == null ? null : entry.CurrentValues[propertyName].ToString()
                                        }
                        }));
                }
                else if (entry.State == EntityState.Deleted)
                {
                    logs.AddRange(FieldsToLog(entryType)
                        .Select(propertyName => new AuditLogEntity()
                        {
                            Entity = entry.Entity,
                            Log = new AuditLog()
                            {
                                Discriminator = entryType.ToString(),
                                Action = entry.State.ToString(),
                                Field = propertyName,
                                FromValue = entry.OriginalValues[propertyName] == null ? null : entry.OriginalValues[propertyName].ToString(),
                                ToValue = string.Empty
                            }
                        }));
                }
                else if (entry.State == EntityState.Modified)
                {
                    logs.AddRange(FieldsToLog(entryType)
                        .Where(propertyName => !object.Equals(entry.OriginalValues[propertyName], entry.CurrentValues[propertyName]))
                        .Select(propertyName => new AuditLogEntity()
                        {
                            Entity = entry.Entity,
                            Log = new AuditLog()
                            {
                                Discriminator = entryType.ToString(),
                                Action = entry.State.ToString(),
                                Field = propertyName,
                                FromValue = entry.OriginalValues[propertyName] == null ? null : entry.OriginalValues[propertyName].ToString(),
                                ToValue = entry.CurrentValues[propertyName] == null ? null : entry.CurrentValues[propertyName].ToString()
                            }
                        }));
                }
            }

            //update the who and when columns
            foreach (var entry in ChangeTracker.Entries<IAuditable>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedOn = now;
                    entry.Entity.CreatedBy = CurrentUser;
                    entry.Entity.ModifiedOn = now;
                    entry.Entity.ModifiedBy = CurrentUser;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedOn = now;
                    entry.Entity.ModifiedBy = CurrentUser;
                }
            }

            //write the list of user changes to the log table
            //NOTE: the ID for the object is NOT Populated till after the first "save changes"
            foreach (var auditLog in logs)
            {
                auditLog.Log.DiscriminatorId = auditLog.Entity.Id;
                auditLog.Log.DiscriminatorTenantId = auditLog.Entity.TenantId;
                auditLog.Log.ChangedBy = CurrentUser;
                auditLog.Log.ChangedOn = now;
                auditLog.Log.TenantId = TenantId;
                this.AuditLog.Add(auditLog.Log);
            }
        }

        /// <summary>
        /// We are useing reflection to figure out what fields we need to worry about
        ///   We are caching the results of the reflection so that we only need to do this
        ///   one time per entity type
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        private IEnumerable<string> FieldsToLog(Type entityType)
        {
            if (this.AuditInfo == null)
                this.AuditInfo = new Dictionary<Type, List<string>>();

            if (!this.AuditInfo.ContainsKey(entityType))
            {
                var auditPropertyInfo = new List<string>();
                var auditFields = typeof(IAuditable).GetProperties().Select(x => x.Name).ToList();
                foreach (var property in entityType.GetProperties())
                {
                    if (property.GetCustomAttributes(typeof(AuditIgnoreAttribute), false).FirstOrDefault() == null
                        && !auditFields.Contains(property.Name) && property.Name != "IdScope")
                    {
                        auditPropertyInfo.Add(property.Name);
                    }
                }
                this.AuditInfo.Add(entityType, auditPropertyInfo);
            }

            return this.AuditInfo[entityType];
        }
    }
}
