using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class ProductionUpdateWithAiModel
    {
        public int OffLinePanels { get; set; }
        public int AIAnomalyPanels { get; set; }
        public List<PanelAnomalyDetail> AnomalyDetails { get; set; } = null!;
        public List<LowStockProduct> LowProductStock { get; set; } = null!;
    }

    public class PanelAnomalyDetail
    {
        public int PanelId { get; set; }
        public bool IsAnomaly { get; set; }
        public string Severity { get; set; } = null!;
        public decimal? LatestPower { get; set; }
        public double Deviation { get; set; }
        public double AvgPower { get; set; }

        public double DropPercentage { get; set; }
    }

    public class LowStockProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Stock { get; set; }
    }
}
