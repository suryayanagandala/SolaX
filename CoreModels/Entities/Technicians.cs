using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("Technicians")]
    public class Technicians
    {
        [Key]
        [Column("TechnicianId")]
        public int TechnicianId { get; set; }

        [Required,MaxLength(6)]
        public required string UserId {  get; set; }

        [Required,MaxLength(50)]
        [Column("Specialization")]
        public required string Specialization { get; set; }

        [Required, Range(0, 60)]
        [Column("ExperienceYears")]
        public decimal ExperienceYears { get; set; }

        [Required,MaxLength (100)]
        [Column("Qualification")]
        public required string Qualification {  get; set; }

        [Column("Descipline")]
        public string? Descipline {  get; set; }

        [Required]
        [Column("Salary")]
        public decimal Salary {  get; set; }

        [Required]
        [Column("Service_Price")]
        public decimal Service_Price { get; set; }

        [Column("IsAvailable")]
        public bool IsAvailable { get; set; } = true;

        [Column("IsActiveDeliveries")]
        public int? IsActiveDeliveries { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive {  get; set; }=true;

        [ForeignKey("UserId")]
        public Users User { get; set; }=null!;


        public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

        public ICollection<InstallationTechnician> Installations { get; set; }=new List<InstallationTechnician>();

        public ICollection<MaintenanceTechnicians> Maintenance { get; set; }= new List<MaintenanceTechnicians>();
    }
}
