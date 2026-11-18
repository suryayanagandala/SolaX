using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class SoftDeleteProductDto
    {
        public int ProductId { get; set; }
        public string Reason { get; set; } = null!;
    }
}
