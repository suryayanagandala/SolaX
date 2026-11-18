using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace BusinessLayer
{
    public class InstallationService
    {
        private readonly IUnitOfWork unitOfWork;
        private ISignalRRepository signalRRepository;
        public InstallationService(IUnitOfWork unitOfWork,ISignalRRepository signalRRepository)
        {
            this.unitOfWork=unitOfWork;
            this.signalRRepository=signalRRepository;
        }

    
        public async Task<List<InstallationCreateDto>>GetInsttalationStatus(string userId)
        {
            return await this.unitOfWork.installRepository.GetInstallationStatus(userId);
        }

        public async Task MarkasProgress(string id)
        {
            await this.unitOfWork.installRepository.MarkasProgress(id);
        }

        public async Task<string> CompletedInstallation(InstallationReportDto dto)
        {
            var technician= await unitOfWork.installRepository.UploadInstallationReports(dto);
            var data = this.unitOfWork.RoleRepository.GetUsersFromUser("Admin");
            foreach (var item in data)
            {
                var notification = new Notifications
                {
                    UserId = item.UserId!,
                    Type = NotificationType.Installation,
                    Title = "Installation completed",
                    Message = $"Installation Id{dto.InstallationId} has been installation successfully by {technician}"
                };

                await unitOfWork.noticationRepositry.AddNotification(notification);
                

                var dtos = new NotificationDto
                {

                    UserId = item.UserId!,
                    NotificationId = notification.NotificationId,
                    NotificationType = notification.Type.ToString()!,
                    Title = notification.Title,
                    Message = notification.Message,
                    IsRead = false,
                    CreatedAt = notification.CreatedAt,
                };
                await this.signalRRepository.SendNotificationAdmin(notification.UserId, dtos);

                
            }

            var techncianmsg = new Notifications
            {
                UserId = technician!,
                Type = NotificationType.Installation,
                Title = "Installation completed",
                Message = $"Installation Id{dto.InstallationId} has been installation successfully by {technician}",
                CreatedAt=TimeUtility.NowIST(),

            };
            await unitOfWork.noticationRepositry.AddNotification(techncianmsg);
            var dtomsg = new NotificationDto
            {

                UserId = techncianmsg.UserId!,
                NotificationId = techncianmsg.NotificationId,
                NotificationType = techncianmsg.Type.ToString()!,
                Title = techncianmsg.Title,
                Message = techncianmsg.Message,
                IsRead = false,
                CreatedAt = techncianmsg.CreatedAt,
            };
            await this.signalRRepository.SendAsyncNotification(techncianmsg.UserId, dtomsg);

            return technician;
                
        }
    }
}
