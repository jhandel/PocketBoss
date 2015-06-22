namespace PocketBoss.Common.Data
{
    public class AuditLogEntity
    {
        public AuditLog Log { get; set; }
        public IAuditable Entity { get; set; }
    }
}