using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class ProductAnalyticsDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PowerGenerated { get; set; }
        public string? Unit { get; set; }
    }
}
