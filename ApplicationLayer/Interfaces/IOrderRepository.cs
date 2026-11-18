using CoreModels.DTO_S;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IOrderRepository
    {
        Task<CreateOrderResponse> PlaceOrder(OrderCreateDto dto);


        Task<IEnumerable<OrderDto>>GetOrderHistory(string userId);

        Task<OrderDto> GetOrderDetails(string userId,int orderId);

        Task<UserCheckOutDto> GetUserDetails(string userId);

        Task CancelOrder(int orderId, string userId);

        Task<List<TackDelivery>> GetTrackDelivery(int orderId);
        
    }
}
