using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class QrResponseDto
    {
        public string QrCodeImage { get; set; } = null!;
        public int ExpiresIn { get; set; }
    }
}
