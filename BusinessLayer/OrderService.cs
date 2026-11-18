using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class OrderService
    {
        private readonly IUnitOfWork unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<OrderDto>> OrderHistory(string userId)
        {
            return await this.unitOfWork.orderRepository.GetOrderHistory(userId);
        }

        public async Task<OrderDto> GetOrderDetails(string userId,int orderId)
        {
            return await this.unitOfWork.orderRepository.GetOrderDetails(userId, orderId);
        }

        public async Task<CreateOrderResponse> PlaceOrder(OrderCreateDto dto)
        {
            return await this.unitOfWork.orderRepository.PlaceOrder(dto);
        }

        public async Task CancelOrder(int orderId,string userId)
        {
            await this.unitOfWork.orderRepository.CancelOrder(orderId, userId);
        }

        public async Task<UserCheckOutDto> GetUserDetails(string userId)
        {
            return await this.unitOfWork.orderRepository.GetUserDetails(userId);
        }

        

        public async Task<List<TackDelivery>>TrackDelivery(int orderId)
        {
            return await this.unitOfWork.orderRepository.GetTrackDelivery(orderId);
        }
    }
}
