using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class ComboResposneDto
    {
        public ProductDto Battery { get; set; } = null!;
        public ProductDto Inverter { get; set; }= null!;
        public string Reason { get; set; } = null!;
    }
}
