using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("MaintenanceReports")]
    public class MaintenanceReports
    {
        [Key]
        [Column("ReportId")]
        public int ReportId { get; set; }

        [Required, StringLength(18)]
        [Column("RequestId")]
        public required string RequestId { get; set; }

        [Column("Notes")]
        public string? Notes { get; set; }

        [Column("Image_Url")]
        public string? Image_Url { get; set; }

        [Column("ReplacedParts")]
        public required string ReplacedParts { get; set; }

        [Column("CreatedAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }


        [ForeignKey("RequestId")]
        public MaintenanceRequests Request{ get; set; } = null!;
    }
}
