using CoreModels.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class TechnicianCreateDto
    {

        //public string UserId { get; set; } = null!;
        public Specialization Specialization { get; set; }
        public Qualification Qualification { get; set; }
        public Discipline Discipline { get; set; }
        public decimal ExperienceYears { get; set; }
        public decimal Salary { get; set; }
        public decimal ServicePrice { get; set; }

        public string? EmailId {  get; set; }   

        public string? PhoneNumber {  get; set; }

        public string? Address {  get; set; }

        public string? FullName { get; set; }
    }
}
