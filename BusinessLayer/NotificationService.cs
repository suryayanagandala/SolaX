using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
using CoreModels.Entities;
using iText.Kernel.Pdf.Tagutils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class NotificationService
    {
        private IUnitOfWork unitOfWork;
        public NotificationService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task AddNotification(Notifications notifications)
        {
            await this.unitOfWork.noticationRepositry.AddNotification(notifications);
        }

        public async Task<List<NotificationDto>> GetNotifiations(string userId)
        {
            return await this.unitOfWork.noticationRepositry.GetNotifiationDtos(userId);
        }

        public async Task<int> CountUnreadNotifications(string userId)
        {
            var count=await this.unitOfWork.noticationRepositry.GetCountOfUnreadNotifications(userId);
            return count;
        }

        public async Task MarAsRead(int notificationId)
        {
            await this.unitOfWork.noticationRepositry.MarkAsRead(notificationId);
        }

        public async Task MarkAllAsRead(string userId)
        {
            await this.unitOfWork.noticationRepositry.MarkAllAsRead(userId);
        }

        public async Task<List<NotificationDto>> GetUnreadNotifications(string userId)
        {
           return await this.unitOfWork.noticationRepositry.GetUnreadMessages(userId);
        }


        //public async Task<List<string>> GetAllAdmins()
        //{
        //    return await this.unitOfWork.noticationRepositry.SendAlertNotificationsForAdmins();
        //}
        
    }

}
