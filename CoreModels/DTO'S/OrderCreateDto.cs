using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class OrderCreateDto
    {
        public string UserId { get; set; } = null!;

    
        public bool IsBuyNow { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }

        public string DeliveryAddress { get; set; } = null!;
        public string DeliveryName { get; set; } = null!;
        public string DeliveryPhone { get; set; } = null!;

        public DateTimeOffset? CreatedAt { get; set; } 
    }
}
