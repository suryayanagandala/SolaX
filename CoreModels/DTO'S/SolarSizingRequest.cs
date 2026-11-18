using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class SolarSizingRequest
    {
        public double? MonthlyBill {  get; set; }
        public double? MonthlyUnits {  get; set; }
        public string Location { get; set; } = null!;
        public double RoofSize {  get; set; }
    }
}
