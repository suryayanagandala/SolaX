using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("InstallationReports")]
    public class InstallationReports
    {
        [Key]
        [Column("ReportId")]
        public int ReportId { get; set; }

        [Required,StringLength(18)]
        [Column("InstallationId")]
        public required string InstallationId { get; set; }

        [Column("Notes")]
        public string? Notes { get; set; }

        [Column("Image_Url")]
        public string? Image_Url { get; set; }

        [Column("CheckList")]
        public string? CheckList { get; set; }

        [Column("CreatedAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }


        [ForeignKey("InstallationId")]
        public Installations Installation { get; set; } = null!;
    }
}
