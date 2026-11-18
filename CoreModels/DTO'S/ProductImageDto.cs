using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class ProductImageDto
    {
        public int  ProductId { get; set; }
        public string ImageUrl { get; set; } = null!;

    }

    public class ProductImageForDelivery
    {
        public string ProductName { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
    }
}
