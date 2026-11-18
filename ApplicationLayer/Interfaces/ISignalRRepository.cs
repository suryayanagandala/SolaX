using CoreModels.DTO_S;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface ISignalRRepository
    {
        Task SendAsyncNotification(string userId, NotificationDto notificationDto);

        Task SendNotificationAdmin(string userId,NotificationDto notificationDto);

        Task SendNotificationForSolarPanles(string userId, SolarPerformanceDto data);

        Task SendAlertsMessages(PanelAnomalyDetail panelAnomalyDetail);
    }
}
