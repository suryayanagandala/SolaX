using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;

        public DateTimeOffset? DeliveryDate { get; set; }

        public ICollection<OrderItemCreateDto> Items { get; set; } = new List<OrderItemCreateDto>();

        public ICollection<PaymentDto> Payment { get; set; } = new List<PaymentDto>();


    }
}
