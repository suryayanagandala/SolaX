using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class EmailDto
    {
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
