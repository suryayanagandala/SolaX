using CoreModels.Enums;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    

    [Table("Notifications")]
    public class Notifications
    {
      

        [Key]
        [Required]
        public int  NotificationId { get; set; }

        [Required,StringLength(6)]
        public required string UserId { get; set; }
        public required string Message { get; set; }

        public string? Title { get; set; }
        public NotificationType? Type { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsActive { get; set; } = true;

        [ForeignKey("UserId")]
        public Users User { get; set; } = null!;
    }
}
