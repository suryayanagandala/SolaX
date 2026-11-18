using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    

    [Table("Installations")]
    public class Installations
    {
        [Key]
        [Required,StringLength(18)]
        [Column("InstallationId")]
        public required  string InstallationId { get; set; }

        [Required]
        [Column("OrderId")]
        public int OrderId { get; set; }

        [Required]
        [Column("CreatedAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [Required]
        [Column("Status")]
        public required string Status { get; set; }

        [Column("CompletedDate")]
        public DateTimeOffset? CompletedDate { get; set; }

        [Column("Installation_Price")]
        public decimal? Installation_Price { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("IsPanelCreated")]
        public bool IsPanelCreated {  get; set; }
        
        [Column("DeliveryId")]
        public int DeliveryId { get; set; }

        [ForeignKey("OrderId")]
        public Orders Order { get; set; } = null!;

        [ForeignKey("DeliveryId")]
        public Delivery delivery { get; set; } = null!;

        public ICollection<InstallationTechnician> installationTechnicians { get; set; }= new List<InstallationTechnician>();

        public ICollection<InstallationReports> Reports { get; set; } = new List<InstallationReports>();

        public ICollection<MaintenanceRequests> MaintenanceRequest { get; set;} = new List<MaintenanceRequests>();

    }
}
