using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class InstallationCreateDto
    {
        public int OrderId { get; set; }
        public int DeliveryId { get; set; }
        public DateTimeOffset DeliveredDate { get; set; }

        public DateTimeOffset CompletedDate {  get; set; } 
        
        public decimal InstallationPrice {  get; set; } 

        public int TechnicianId {  get; set; }  

        public decimal ServicePrice { get; set; }

        public string InstallationId { get; set; } = string.Empty;

        public string InstallationStatus { get; set; } = string.Empty;

        public string Actions {  get; set; } = string.Empty;    
    }
}
