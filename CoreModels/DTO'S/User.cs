using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class User
    {
        public string? UserId { get; set; }
        public string? FullName { get; set; }
        public string? EmailId { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}
