using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class ProductReviewsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductReviewsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }

        public async Task<IEnumerable<ProductReviewResponseDto>> GetAllReviewByPrduct(int productId)
        {
            return await _unitOfWork.productReviewRepository.GetAllReviewByPrduct(productId);
        }

       public async Task<AddProductReviewDto> AddProductReview(string userId, AddProductReviewDto dto)
        {
            return await _unitOfWork.productReviewRepository.AddProductReview(userId, dto);
        }

        public async Task<AddProductReviewDto> UpdateProductReview(string userId,int productId,AddProductReviewDto dto)
        {
            return await _unitOfWork.productReviewRepository.UpdateProductReview(userId, productId, dto);   
        }

        public async Task DeleteProductReview(string userId, int productId)
        {
            await _unitOfWork.productReviewRepository.DeleteProductReview(userId, productId);
        }
    }
}
