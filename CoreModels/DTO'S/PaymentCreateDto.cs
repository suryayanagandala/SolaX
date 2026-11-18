using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class PaymentCreateDto
    {
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; } = null!;

        public string? UserId { get; set; }
    }
}
