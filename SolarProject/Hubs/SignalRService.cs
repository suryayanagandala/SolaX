using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
using Microsoft.AspNetCore.SignalR;

namespace SolarProject.Hubs
{
    public class SignalRService : ISignalRRepository
    {
        private readonly IHubContext<NotificationHub> hubContext;

        public SignalRService(IHubContext<NotificationHub> hubContext)
        {
            this.hubContext = hubContext;
        }


        public async Task SendAsyncNotification(string userId, NotificationDto notificationDto)
        {
            await hubContext.Clients.User(userId.ToString()).SendAsync("NewNotification", new NotificationDto
            {
                UserId = userId,
                NotificationId = notificationDto.NotificationId,
                NotificationType = notificationDto.NotificationType,
                Title = notificationDto.Title,
                Message = notificationDto.Message,
                IsRead = false,
                CreatedAt = notificationDto.CreatedAt,

            });
        }

        public async Task SendNotificationAdmin(string userId,NotificationDto notificationDto)
        {
            await hubContext.Clients.Group("Admins").SendAsync("NewNotification", new NotificationDto
            {
                UserId = userId,
                NotificationId = notificationDto.NotificationId,
                NotificationType = notificationDto.NotificationType,
                Title = notificationDto.Title,
                Message = notificationDto.Message,
                IsRead = false,
                CreatedAt = notificationDto.CreatedAt,

            });
        }

        public async Task SendNotificationForSolarPanles(string userId, SolarPerformanceDto data)
        {
            await hubContext.Clients.User(userId.ToString()).SendAsync("SolarPerformance", new
            {
                data.UserId,
                data.CurrentFlow,
                data.Generated_at,
                data.PanelId,
                data.Power,
                data.Efficiency,
                data.Voltage,

            });
        }


        public async Task SendAlertsMessages(PanelAnomalyDetail panelAnomalyDetail)
        {
            await hubContext.Clients.Groups("Admins").SendAsync("AlertMessages", new
            {
                panelAnomalyDetail
            });
        }
    }
}
