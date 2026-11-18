using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class WeatherResponse
    {
        public List<WeatherItem> list { get; set; } = null!;
    }

    public class WeatherItem
    {
        public MainData main { get; set; } = null!;
        public Clouds clouds { get; set; } = null!;
        public string dt_txt { get; set; } = null!;
    }

    public class MainData
    {
        public double temp { get; set; }
        public double humidity { get; set; }
    }

    public class Clouds
    {
        public double all { get; set; } // cloud %
    }



    public class HourlyPrediction
    {
        public string? Time { get; set; }
        public double Power { get; set; }
    }

    public class PredictionWithTime
    {
        public List<double> Predictions { get; set; }=null!;
        public List<DateTime> ForecastTimes { get; set; } = null!;
    }
}
