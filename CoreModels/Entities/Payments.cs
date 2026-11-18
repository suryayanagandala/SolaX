using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
  
  

    [Table("Payments")]
    public class Payments
    {
        [Key]
        [Column("PaymentId")]
        public int PaymentId { get; set; }

        [Required]
        [Column("OrderId")]
        public int OrderId { get; set; }

        [Required]
        [Column("PaymentMethod")]
        public required string PaymentMethod { get; set; }

        [Required]
        [Column("PaymentStatus")]
        public required string PaymentStatus { get; set; }

        [Required,StringLength(15)]
        [Column("TransactionId")]
        public required string TransactionId { get; set; }

        [Required]
        [Column("Amount")]
        public decimal Amount { get; set; }

        [Column("PaymentDate")]
        public DateTimeOffset PaymentDate { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [ForeignKey("OrderId")]
        public Orders Order { get; set; } = null!;
        
    }
}
