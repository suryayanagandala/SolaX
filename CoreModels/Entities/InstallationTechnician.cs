using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("InstallationTechnicians")]
    public class InstallationTechnician
    {
        [Key]
        [Column("Installation_Tech_Id")]
        public int Installation_Tech_Id { get; set; }  

        [Required,StringLength(18)]
        [Column("InstallationId")]
        public required string InstallationId { get; set; }

        [Required]
        [Column("TechnicianId")]
        public int TechnicianId { get; set; }

        [Required]
        [Column("Service_Price_At_Time")]
        public decimal ServicePrice { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; }=true;


        [ForeignKey("InstallationId")]
        public Installations Installation { get; set; } = null!;

        [ForeignKey("TechnicianId")]
        public Technicians Technician { get; set; }=null!;
    }
}
