using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class SolarSizingResponse
    {
        public double RequiredKW { get; set; }
        public int NumberOfPanels { get; set; }

        public double MinBatteryKWh { get; set; }
        public double MaxBatteryKWh { get; set; }
        public double InverterKW { get; set; }
        public double EstimatedMonthlySavings { get; set; }
        public double EstimatedYearlySavings { get; set; }
        public double EstimatedSystemCost { get; set; }
        public double ROIYears { get; set; }
        public string Recommendation { get; set; } = null!;
    }
}
