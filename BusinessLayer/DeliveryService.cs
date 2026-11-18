using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class DeliveryService
    {
        private readonly IUnitOfWork unitOfWork;
        public DeliveryService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<string> MarkAsCompletedDelivery(int deliveryId)
        {
           return await this.unitOfWork.deliveryInterface.MarkAsCompletedDelivery(deliveryId);
        }

        public async Task MarkAsOutOfDelivery(int deliveryId)
        {
            await this.unitOfWork.deliveryInterface.MarkAsOutOfDelivery(deliveryId);
        }

        public async Task<List<DeliveryDto>>GetOutForDeliveryStock(string userId)
        {
            return await this.unitOfWork.deliveryInterface.GetMarkoutDelieries(userId);
        }

        public async Task<List<DeliveryDto>>GetDeliveredOrders(string userId)
        {
            return await this.unitOfWork.deliveryInterface.GetDeliveredOrders(userId);
        }


        public async Task SendNotificationtoInstallers()
        {
            var userIds = await unitOfWork.deliveryInterface.AssignedRemainingDeliveries();

            if (userIds == null || !userIds.Any())
                return;

            foreach (var userId in userIds.Distinct())
            {
                var message = new Notifications
                {
                    UserId = userId,
                    Type = NotificationType.Delivery,
                    Title = "New Installation",
                    Message = "New Delivery assigned to you",
                    IsActive = true
                };

                await unitOfWork.noticationRepositry.AddNotification(message);

                var dto = new NotificationDto
                {
                    UserId = userId,
                    NotificationId = message.NotificationId,
                    NotificationType = message.Type.ToString()!,
                    Title = message.Title,
                    Message = message.Message,
                    IsRead = false,
                    CreatedAt = message.CreatedAt
                };

                //await unitOfWork.signalRRepository.SendAsyncNotification(userId, dto);
            }
        }

    }
}
