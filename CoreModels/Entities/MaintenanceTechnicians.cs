using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("MaintenanceTechnicians")]
    public class MaintenanceTechnicians
    {
        [Key]
        [Column("MaintenanceId")]
        public int MaintenanceId { get; set; }

        [Required, StringLength(8)]
        [Column("RequestId")]
        public required string RequestId { get; set; }

        [Required]
        [Column("TechnicianId")]
        public int TechnicianId { get; set; }

        [Required]
        [Column("Service_Price_At_Time")]
        public decimal ServicePrice { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;


        [ForeignKey("RequestId")]
        public MaintenanceRequests GetMaintenance { get; set; } = null!;

        [ForeignKey("TechnicianId")]
        public Technicians Technician { get; set; } = null!;
    }
}
