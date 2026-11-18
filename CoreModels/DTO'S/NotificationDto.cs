using CoreModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class NotificationDto
    {
        
        public string UserId { get; set; } = null!;
        public int NotificationId { get; set; }
        public string UserName { get; set; } = null!;
        
        public string NotificationType {  get; set; } = null!;  

        public string? Title {  get; set; }
        public string Message { get; set; } = null!;

        public bool IsRead {  get; set; }

   

        public DateTimeOffset CreatedAt { get; set; }
    }
}
