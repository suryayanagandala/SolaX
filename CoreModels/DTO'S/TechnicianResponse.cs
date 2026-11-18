using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class TechnicianResponse
    {
        public int TechnicianId {  get; set; }

        public string UserId {  get; set; }=string.Empty;

        public string TechnicianName { get; set; } = null!;

        public string Specifications {  get; set; }=null!;

        public int ActiveDeliveries {  get; set; }

        public bool IsAvaialble {  get; set; }

        public decimal Salary { get; set; }

        public decimal ExperienceYears { get; set; }

        public string? Qualifications { get; set; }
         
        public string? Specialization { get; set; }

        public decimal ServicePrice { get; set; }


        
    }
}
