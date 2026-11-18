using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class RecommendationResponseDto
    {
        public List<ProductDto> Accessories { get; set; } = new();
        public List<ProductDto> Batteries { get; set; } = new();
        public List<ProductDto> Inverters { get; set; } = new();
        public string Message { get; set; } = string.Empty;

        public string? AIExplanation { get; set; }
    }
}
