using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class PredictionRequest
    {
        
            [JsonPropertyName("irradiance")]
            public double Irradiance { get; set; }

            [JsonPropertyName("temperature")]
            public double Temperature { get; set; }

            [JsonPropertyName("cloudCover")]
            public double CloudCover { get; set; }

            [JsonPropertyName("humidity")]
            public double Humidity { get; set; }

            public DateTime Time { get; set; }

            public int Hour { get; set; }

    }

    public class AiPredictionResponse
    {
        public bool Status { get; set; }
        public double PredictedPower { get; set; }
    }
}
