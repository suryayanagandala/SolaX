using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("SolarPerformance")]
    public class SolarPerformance
    {
        [Key]
        public int PerformanceId { get; set; }

        [Required,StringLength(6)]
        public required string UserId { get; set; }

        public int PanelId {  get; set; }
        public decimal? Voltage { get; set; }
        public decimal? CurrentFlow { get; set; }
        public decimal? Power { get; set; }
        public decimal? Efficiency { get; set; }
        public DateTimeOffset Generated_at { get; set; }

        public int Temperature {  get; set; } 
        public bool IsActive { get; set; } = true;

        [ForeignKey("UserId")]
        public Users User { get; set; } = null!;

        [ForeignKey("PanelId")]
        public Panel Panel { get; set; } = null!;


    }
}
