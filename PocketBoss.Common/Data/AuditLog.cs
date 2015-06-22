using PocketBoss.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PocketBoss.Common.Data
{
        [Serializable]
    public class AuditLog : MultiTenantEntityBase
    {
        #region Relationship Keys
        [Required]
        public string Discriminator { get; set; }
        [Required]
        public long DiscriminatorId { get; set; }
        [Required]
        public Guid DiscriminatorTenantId { get; set; }
        #endregion

        #region Properties
        public string Field { get; set; }
        public string Action { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set; }
        public DateTime ChangedOn { get; set; }
        public string ChangedBy { get; set; }
        #endregion
    }

}
