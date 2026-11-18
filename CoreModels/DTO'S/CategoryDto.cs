using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public string? Slug { get; set; }
        public int? ParentId { get; set; }

        public string? DeletedReason { get; set; }
        public bool IsActive { get; set; }

        public IFormFile? Image { get; set; }

        public string? ImageUrl { get; set; }

        public string? SelectedChildSlug { get; set; }
        public List<CategoryDto>? Children { get; set; } = new List<CategoryDto>();
        public List<ProductDto>? ProductsDto { get; set; } = new List<ProductDto>();

       
    }
}
