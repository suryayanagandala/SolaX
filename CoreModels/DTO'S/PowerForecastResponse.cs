using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class PowerForecastResponse
    {
        public List<HourlyPrediction> HourlyPredictions { get; set; } = null!;
        public double AveragePower { get; set; }
        public double PeakPower { get; set; }
        public int PeakHour { get; set; }
        public List<string>? Alerts { get; set; }
      
    }
}
