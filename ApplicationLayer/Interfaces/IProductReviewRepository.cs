using CoreModels.DTO_S;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IProductReviewRepository
    {
        Task<IEnumerable<ProductReviewResponseDto>> GetAllReviewByPrduct(int productId);

        Task<AddProductReviewDto> AddProductReview(string userId,AddProductReviewDto dto);

        Task<AddProductReviewDto> UpdateProductReview(string userId, int productId, AddProductReviewDto dto);

        Task  DeleteProductReview(string userId,int productId);


    }
}
