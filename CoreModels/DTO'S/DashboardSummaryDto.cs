using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class DashboardSummaryDto
    {
        public decimal? TodayTotal { get; set; }
        public decimal? MonthlyPower { get; set; }
        public decimal? LifeTimePower { get; set; }
        public decimal? LifeTimeSavings { get; set; }

        public decimal? AverageEfficiency { get; set; }

    }
}
