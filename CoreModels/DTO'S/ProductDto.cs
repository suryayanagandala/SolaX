using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int CapacityValue { get; set; }
        public string Capacity { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal Warranty_Years { get; set; }
        public int Stock { get; set; }

        public bool IsActive { get; set; }
        public string Description { get; set; } = null!;

        public DateTimeOffset? CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public string? DeletedReason { get; set; } = null!;

        public int CategoryId { get; set; }

        public string? CategoryName { get;set ; }

        public double? AverageRating { get; set; }
        public int? RatingCount { get; set; }

        public string? ImageUrl { get; set; }
        public CategoryDto Category { get; set; } = null!;

        public List<ProductImageResponseDto> ProductImages { get; set; } = new();

        public List<ProductReviewResponseDto> productReviews { get; set; } = new();

    }
}
