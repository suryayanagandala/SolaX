using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class HomeService
    {
        private readonly IUnitOfWork unitOfWork;

        public HomeService(IUnitOfWork unitOfWork)
        {
         this.unitOfWork = unitOfWork;   
        }

        public async Task<List<CategoryDto>> GetHomeCategories()
        {
            return await this.unitOfWork.homeRepository.GetHomeCategories();
        }

        public async Task<List<CategoryDto>> GetHomePackage()
        {
            return await this.unitOfWork.homeRepository.GetHomePackage();
        }

        public async Task<CategoryDto> GetCategoriesWiseBySlug(string slug)
        {
            return await this.unitOfWork.homeRepository.GetProductBySlug(slug);
        }

        public async Task<ProductDto> ViewProductDetails(int productId)
        {
            return await this.unitOfWork.homeRepository.ViewProductDetails(productId);
        }

        public async Task<ProductAnalyticsDto> GetAnalyticsOfProduct(int productId)
        {
            return await unitOfWork.homeRepository.GetProductAnalytics(productId);
        }
    }
}
