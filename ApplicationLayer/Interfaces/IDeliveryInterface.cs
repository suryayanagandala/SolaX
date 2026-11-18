using CoreModels.DTO_S;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public  interface IDeliveryInterface
    {
        Task<string> MarkAsCompletedDelivery(int deliveryId);

        Task MarkAsOutOfDelivery(int deliveryId);

        Task<List<string>> AssignedRemainingDeliveries();

        Task<List<DeliveryDto>> GetMarkoutDelieries(string userId);

        Task<List<DeliveryDto>> GetDeliveredOrders(string userId);

        Task SentDelayMessagesForDeliveryData();

    }
}
