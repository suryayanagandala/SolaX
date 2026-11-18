using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class MonthlyCompletedJobsDto
    {
        public string Month { get; set; } = string.Empty;
        public int CompletedJobs { get; set; }
    }
}
