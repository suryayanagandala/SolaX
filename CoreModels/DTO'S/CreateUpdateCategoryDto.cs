using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class CreateUpdateCategoryDto
    {
        public int? CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public string? Slug {  get; set; }
        public int? ParentId { get; set; }

        public IFormFile? Image { get; set; }
    }
    
}
