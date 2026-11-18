using CoreModels.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
   

    [Table("Orders")]
    public class Orders
    {
        [Key]
        [Column("OrderId")]
        public int OrderId {  get; set; }

        [Required,StringLength(6)]
        [Column("UserId")]
        public required string UserId {  get; set; }

        [Column("OrderDate")]
        public DateOnly OrderDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Required,MaxLength(50)]
        [Column("Status")]
        public required OrderStatus Status {  get; set; }

        [Required]
        [Column("TotalAmount")]
        public decimal TotalAmount {  get; set; }

        [Required,MaxLength(1000)]
        [Column("DeliveryAddress")]
        public required string DeliveryAddress {  get; set; }

        [Required]
        public required string PhoneNumber { get; set; }

        [Required]

        public required string FullName { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive {  get; set; }

        [Column("CreatedAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public Users User { get; set; } = null!;


        public ICollection<OrderItems> OrderItem { get; set; } = new List<OrderItems>();

        public ICollection<Payments> Payment { get; set; } = new List<Payments>();

        //public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
        public Delivery? Delivery { get; set; }

        public ICollection<Installations> Installation { get; set; } = new List<Installations>();

        public ICollection<Invoices> Invoices { get; set; }= new List<Invoices>();
    }
}
