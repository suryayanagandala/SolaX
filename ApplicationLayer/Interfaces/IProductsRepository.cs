using CoreModels.DTO_S;
using CoreModels.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IProductsRepository
    {
        Task AddProduct(ProductResponse dto);

        Task UpdateProduct(int productId, ProductResponse dto);

        Task<ProductDto> GetProductById(int productId);

        Task<IEnumerable<ProductDto>> GetAllPrducts();

        Task DeleteProductById(int productId);

        Task SoftDeleteProduct(SoftDeleteProductDto dto);

        Task RestoreProduct(int productId);
        Task AddImage(int productId,List<IFormFile> images);

        Task UpdateProductImage(int imageId, IFormFile image);

        Task<IEnumerable<ProductImageResponseDto>> GetIamges(int productId);

        Task DeleteImage(int imageId);


    }
}
