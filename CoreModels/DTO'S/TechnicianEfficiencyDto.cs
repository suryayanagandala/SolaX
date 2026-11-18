using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class TechnicianEfficiencyDto
    {
        public int TotalJobs { get; set; }
        public int CompletedJobs { get; set; }
        public double Efficiency { get; set; }
    }
}
