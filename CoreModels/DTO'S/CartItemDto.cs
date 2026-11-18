using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class CartItemDto
    {
        public int ProductId { get; set; }

        public int CartItemId { get; set; }

        public string ProductName { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        public decimal Price { get; set; }

        public string Capacity {  get; set; } = null!;

        public string Description {  get; set; } = null!;

        public string Type {  get; set; }=null!;
        
        public string Brand {  get; set; } = null!; 

        public int Quantity { get; set; }

        
    }
}
