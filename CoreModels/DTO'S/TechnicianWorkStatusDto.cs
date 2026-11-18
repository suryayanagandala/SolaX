using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{

    public class TechnicianWorkStatusDto
    {
        public int Assigned { get; set; }
        public int Pending { get; set; }
        public int Completed { get; set; }
    }
}
