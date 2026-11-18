using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class PredicitResponse
    {
        [JsonPropertyName("predictedPower")]
        public double PredictPower { get; set; }
    }
}
