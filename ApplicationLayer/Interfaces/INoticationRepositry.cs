using CoreModels.DTO_S;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public  interface INoticationRepositry
    {
        Task<Notifications>AddNotification(Notifications notification);

        Task<List<NotificationDto>> GetNotifiationDtos(string userId);


        Task<List<NotificationDto>> GetUnreadMessages(string userId);

        Task<int> GetCountOfUnreadNotifications(string userId);

        Task MarkAsRead(int notificationId);

        Task MarkAllAsRead(string userId);

        Task<List<string>> SendAlertNotificationsForAdmins( int panelId, string severity, double dropPercentage);
    }
}
