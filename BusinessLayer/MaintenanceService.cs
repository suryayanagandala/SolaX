using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class MaintenanceService
    {
        private readonly IUnitOfWork unitOfWork;
        private ISignalRRepository signalRRepository;
        public MaintenanceService(IUnitOfWork unitOfWork, ISignalRRepository signalRRepository)
        {
            this.unitOfWork = unitOfWork;
            this.signalRRepository = signalRRepository;
        }

        public List<DeliveryDto> GetMaintenanceForUser(string userId)
        {
            return this.unitOfWork.maintenanceRepository.GetMaintenanceStatus(userId);
        }

        public async Task RaiseMaintenanaceRequest(MaintenanceRequestCreateDto dto)
        {
            await this.unitOfWork.maintenanceRepository.RaiseComplete(dto);
        }

        public  async Task<List<MaintenanceRequestDto>> GetMaintenanceJods(string userId)
        {
            return await unitOfWork.maintenanceRepository.maintenanceRequestDtos(userId);
        }

        public async Task UploadMaintennaceStatus(MaintenanceReportDto maintenanceReportDto)
        {
            var list=await this.unitOfWork.maintenanceRepository.CompletedMaintenanceTask(maintenanceReportDto);
            if (list.Count >= 4)
            {
                var userId = list[1];
                var technicianId = list[0];
                var orderId= list[2];
                var requestId= list[3];

                var messages = new Notifications
                {
                    UserId = userId,
                    Type = NotificationType.Maintenance,
                    Title = "Maintenance Update",
                    Message = $"Maintenance completed of {orderId} having requestId {requestId}"
                };
                await this.unitOfWork.noticationRepositry.AddNotification(messages);
                await this.unitOfWork.SaveAsync();
                var notificationDto = new NotificationDto
                {
                    UserId = userId,
                    NotificationType = messages.Type.ToString()!,
                    Title = messages.Title,
                    Message = messages.Message,
                    IsRead = false,
                    CreatedAt = DateTime.Now,
                    NotificationId = messages.NotificationId,
                };

                await signalRRepository.SendAsyncNotification(userId, notificationDto);
                await signalRRepository.SendAsyncNotification(technicianId, notificationDto);
                await signalRRepository.SendNotificationAdmin(userId, notificationDto);
            }

        }

        public async Task<List<object>>GetCompletedMaintenanaces(string userId)
        {
            return await this.unitOfWork.maintenanceRepository.GetCompletedMaintenances(userId);
        }
    }
}
