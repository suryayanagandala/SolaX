using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;         
        public string UserRole { get; set; }=null!;

        public string TableName { get; set; } = null!;
        public string Action { get; set; } = null!;

        public string OldValues { get; set; } = null!;
        public string NewValues { get; set; } = null!;     

        public DateTime ChangedAt { get; set; }
    }
}
