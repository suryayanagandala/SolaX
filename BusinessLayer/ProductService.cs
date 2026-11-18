using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
using CoreModels.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class ProductService
    {
        private readonly IUnitOfWork unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task AddProducts(ProductResponse productCreateDto)
        {
            await this.unitOfWork.ProductsRepository.AddProduct(productCreateDto);
        }

        public async Task<IEnumerable<ProductDto>> GetAllProducts()
        {
            return await this.unitOfWork.ProductsRepository.GetAllPrducts();

        }

        public async Task<ProductDto>GetProductById(int productId)
        {
            return await this.unitOfWork.ProductsRepository.GetProductById(productId);
        }
        public async Task UpdateProduct(int productId,ProductResponse dto)
        {
            await this.unitOfWork.ProductsRepository.UpdateProduct(productId, dto);
        }

        public async Task DeleteProductById(int productId)
        {
            await this.unitOfWork.ProductsRepository.DeleteProductById(productId);
        }

        public async Task SoftDeleteProduct(SoftDeleteProductDto dto)
        {
            await this.unitOfWork.ProductsRepository.SoftDeleteProduct(dto);
        }

        public async Task RestoreProduct(int productId)
        {
            await this.unitOfWork.ProductsRepository.RestoreProduct(productId);
        }
        public async Task AddProductImages(int productId, List<IFormFile> images)
        {
            await this.unitOfWork.ProductsRepository.AddImage(productId, images);
        }

        public async Task UpdatePrpductImage(int imageId, IFormFile file)
        {
            await this.unitOfWork.ProductsRepository.UpdateProductImage(imageId, file);
        }

       public async Task<IEnumerable<ProductImageResponseDto>> GetIamgesById(int productId)
        {
            return await this.unitOfWork.ProductsRepository.GetIamges(productId);
        }

        public async Task DeleteImageById(int imageId)
        {
            await this.unitOfWork.ProductsRepository.DeleteImage(imageId);
        }



    }
}
