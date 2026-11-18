using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class CartResponseDto
    {
        public int CartId { get; set; }

        public decimal TotalPrice { get; set; }
        public List<CartItemDto> Items { get; set; }=new List<CartItemDto>();

    }
}
