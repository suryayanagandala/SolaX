using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class ComboDto
    {
        public string BatteryName { get; set; } = null!;
        public string InverterName { get; set; }= null!;
        public string Reason { get; set; } = null!;
    }
}
