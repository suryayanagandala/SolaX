using CoreModels.DTO_S;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IMaintenanceRepository
    {
        List<DeliveryDto> GetMaintenanceStatus(string userId);

        Task RaiseComplete(MaintenanceRequestCreateDto dto);

        Task<List<MaintenanceRequestDto>> maintenanceRequestDtos(string userId);

        Task<List<string>> CompletedMaintenanceTask(MaintenanceReportDto maintenanceReportDto);

        Task<List<object>>GetCompletedMaintenances(string userId);

        Task SendNotificationsForDelayMaintenance();
    }
}
