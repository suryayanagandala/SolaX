using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class TechnicianDashboardDto
    {
        public int PendingCount { get; set; }
        public int OutForDeliveryCount { get; set; }
        public int DeliveredTodayCount { get; set; }
        public int TotalActiveDeliveries { get; set; }

        public int AssignedCount { get; set; }
        public int InProgressCount {  get; set; }
        public int CompletedCount {  get; set; }

        public string Specilization { get; set; } = null!;
    }
}
