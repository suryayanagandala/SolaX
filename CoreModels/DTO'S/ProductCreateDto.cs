using CoreModels.Entities;
using CoreModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class ProductResponse
    {
        public string ProductName { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int CapacityValue { get; set; }
        public string Capacity { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal Warranty_Years { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; } = null!;

        public bool IsActive { get; set; }

        public int CategoryId { get; set; }    
    }
}
