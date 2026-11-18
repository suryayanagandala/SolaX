using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class MonthlyProductionDto
    {
        public DateTime Date { get; set; }
        public decimal? TotalPower { get; set; }
    }
}
