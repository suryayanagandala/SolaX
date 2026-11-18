using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("OrderItems")]
    public class OrderItems
    {
        [Key]
        [Column("OrderItemId")]
        public int OrderItemId {  get; set; }

        [Required]
        [Column("OrderId")]
        public int OrderId {  get; set; }

        [Required]
        [Column("ProductId")]
        public int ProductId { get; set; }

        [Required]
        [Column("Quantity")]
        public int Quantity { get; set; }

        [Required]
        [Column("UnitPrice")]
        public decimal UnitPrice { get; set; }

        [Column("TotalPrice")]
        public decimal TotalPrice => Quantity * UnitPrice;

        [ForeignKey("OrderId")]
        public Orders Order { get; set; } = null!;


        [ForeignKey("ProductId")]
        public Products Product {  get; set; } = null!;    

    }
}
