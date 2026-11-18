using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class PaymentDto
    {
        public string PaymentId { get; set; } = null!;
        public int OrderId { get; set; }
        public string PaymentStatus { get; set; } = null!;

        public string PaymentMethod { get; set; } = null!;  

        public DateTimeOffset PaymentDate { get; set; }
        public decimal Amount {  get; set; }
    }
}
