using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class SolarPerformanceDto
    {
        public string UserId { get; set; } = string.Empty;

        public decimal? Voltage {  get; set; }

        public decimal? CurrentFlow {  get; set; }

        public decimal? Power { get; set; }

        public decimal? Efficiency {  get; set; }

        public DateTimeOffset Generated_at { get; set; } 

        public int PanelId { get; set; }
    }
}
