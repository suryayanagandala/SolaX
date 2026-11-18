using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("Invoices")]
    public class Invoices
    {
        [Key]
        public int InvoiceId { get; set; }  
        public int OrderId { get; set; }

        public required  string CustomerName { get; set; }

        public required string CustomerEmail { get; set; }

        public required string PhoneNumber {  get; set; }
        public DateTimeOffset InvoiceDate { get; set; }
        public decimal Amount { get; set; }

        public decimal Tax { get; set; }

        public required string PaymentStatus { get; set; }

        public decimal TotalAmount { get; set; }

        public required string OrderStatus { get; set; }

        public required string PaymentMethod { get; set; }
        public bool IsActive { get; set; } = true;


        [ForeignKey("OrderId")]
        public Orders Order { get; set; } = null!;
    }
}
