using iText.Pdfua.Checkers.Utils.Ua1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class MaintenanceRequestDto
    {
        public string RequestId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal? EstimatedPrice { get; set; }

        public string? CustomerAddress {  get; set; }   

        public string? CustomerPhoneNumber {  get; set; }

        public string? CustomerName { get; set; }

        public int? OrderId {  get; set; }  

        public DateTimeOffset? AssignedDate {  get; set; }
    }
}
