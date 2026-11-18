using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    

    [Table("Delivery")]
    public class Delivery
    {
        [Key]
        [Column("DeliveryId")]
        public int DeliveryId {  get; set; }

        [Required]
        [Column("OrderId")]
        public int OrderId { get; set; }

        
        [Column("AssignedToTechId")]
        public int? AssignedToTechId { get; set; }

        [MaxLength(30)]
        [Column("Status")]
        public string? Status { get; set; } = null!;

        [Column("AssignedAt")]
        public DateTimeOffset AssignedAt { get; set; }

        [Column("DeliveryAt")]
        public DateTimeOffset? DeliveryAt { get; set; }

        [Column("AssignedBy")]
        public string AssignedBy { get; set; } = "Admin";

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; }=true;

        [ForeignKey("OrderId")]
        public Orders Order { get; set; } = null!;

        [ForeignKey("AssignedToTechId")]
        public Technicians? AssignedToTech { get; set; }


        public ICollection<Installations> Installations { get; set; } = new List<Installations>();
    }
}
