using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    

    [Table("MaintenanceRequests")]

    public class MaintenanceRequests
    {
        [Key]
        [Required,StringLength(8)]
        public required string RequestId { get; set; }  

        [Required,StringLength(6)]
        public required string UserId { get; set; }
        public string? Description { get; set; }

        [Required,MaxLength(50)]
        public required string Status { get; set; }


        public decimal? EstimatedPrice { get; set; }


        public DateTimeOffset CreatedAt { get; set; }

        [Required]
        public bool IsActive { get; set; }=true;

        [Required,StringLength(18)]
        public required string InstallationId { get; set; }

        [ForeignKey("UserId")]
        public Users User { get; set; } = null!;

        [ForeignKey("InstallationId")]
        public Installations Installation { get; set; } = null!;


        public ICollection<MaintenanceTechnicians> Maintenances {  get; set; }=new List<MaintenanceTechnicians>();

        public ICollection<MaintenanceReports> MaintenanceReports { get; set;} =new List<MaintenanceReports>();
    }
}
