using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class InvoiceDto
    {
        public int OrderId {  get; set; }

        public string CustomerName { get; set; }= null!;

        public string CustomerEmail { get; set; } = null!;

        public string PhoneNumber { get; set; }=null!;

        public string OrderStatus { get; set; }=null!;

        public DateOnly InvoiceDate { get; set; }

        public decimal Tax {  get; set; }

        public PaymentDto? Payment { get; set; }

    }
}
