using CoreModels.DTO_S;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IHomeRepository
    {
        Task<List<CategoryDto>> GetHomeCategories();

        Task<CategoryDto> GetProductBySlug(string slug);

        Task<List<CategoryDto>> GetHomePackage();

        Task<ProductDto> ViewProductDetails(int productId);

        Task<ProductAnalyticsDto> GetProductAnalytics(int productId);
    }
}
