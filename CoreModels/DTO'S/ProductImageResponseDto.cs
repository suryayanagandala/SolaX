using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class ProductImageResponseDto
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public DateTimeOffset? CreatedAt { get; set; }

        public int? ProductId { get; set; } 
        public bool? IsActive {  get; set; }
        public  DateTimeOffset? UpdatedAt { get; set; }
        
    }
}
