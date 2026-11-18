using CoreModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class CreateOrderResponse
    {
        public int OrderId { get; set; }

        public string OrderStatus { get; set; } = null!; 
    }
}
