using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class AdminDashboardStatsDto
    {
        public int Orders { get; set; }

        public int Users { get; set; }

        public int Products { get; set; }

        public int Employees { get; set; }

        public int TotalEmployess { get; set; }

        public int TotalEnergy { get; set; }

        public int PanlesCount { get; set; }

        public int Installation { get; set; }

        public int Delieveries { get; set; }
    }
}
