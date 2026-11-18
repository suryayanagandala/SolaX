using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using DbAccessaLayer.Infrastructure;
using iText.Bouncycastle.Crypto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using SolarXNUnit.SqLite.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarXNUnit.SqLite.Infrastructure
{
    public class ProductRepositoryTest:SqliteTestBase
    {
        private ProductRepository productRepository;

        private IFormFile CreateFakeFile(string fileName)
        {
            var content = "Dummy file content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            return new FormFile(stream, 0, stream.Length, "Data", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };
        }

        [SetUp]
        public void SetUp()
        {
            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.Setup(x => x.WebRootPath).Returns(Path.GetTempPath());

            productRepository=new ProductRepository(_db,mockEnv.Object);
        }


        [Test]
        public async Task GetAllProducts_Success()
        {
            var result=await productRepository.GetAllPrducts();
            Assert.IsNotEmpty(result);
            foreach (var item in result)
            {
                Assert.IsNotEmpty(item.ProductName);
            }
        }

        //AddProducts
        [Test]
        public void AddProducts_Invalid_CategoryId()
        {
         
            var dto = new ProductResponse
            {
                ProductName="Product1",
                CategoryId = 6,
                Brand = "Tata",
                Type = "Panel",
                CapacityValue = 500,
                Capacity = "500W",
                Price = 15000,
                Warranty_Years = 5,
                Stock = 10,
                Description = "High efficiency solar panel",
            
               
            };

            Assert.ThrowsAsync<NotFoundException>(async()=>await productRepository.AddProduct(dto));
        }

        [Test]
        public void AddProducts_SqlException()
        {
            var ex=Assert.ThrowsAsync<ArgumentNullException>(async () => await productRepository.AddProduct(null));
            
        }

        [Test]
        public async Task AddToProducts_Success()
        {
            var dto = new ProductResponse
            {
                ProductName = "Product1",
                CategoryId = 1,
                Brand = "Tata",
                Type = "Panel",
                CapacityValue = 500,
                Capacity = "500W",
                Price = 15000,
                Warranty_Years = 5,
                Stock = 10,
                Description = "High efficiency solar panel",


            };

            await productRepository.AddProduct(dto);    


        }

        //getProductsById
        [Test]
        public void GetByProductById_FailureId()
        {
            int id = 10;
            Assert.ThrowsAsync<NotFoundException>(async()=>await productRepository.GetProductById(id));
        }

        [Test]
        public async Task GetProductById_Success()
        {
            int id = 1;
            var result=await productRepository.GetProductById(id);
            Assert.IsNotEmpty(result.Capacity);
        }

        //softDelete
        [Test]
        public void DeleteProducts_Failure_Case()
        {
            var dto = new SoftDeleteProductDto
            {
                ProductId = 10,
                Reason="Low stock"
            };

            Assert.ThrowsAsync<NotFoundException>(async()=>await productRepository.SoftDeleteProduct(dto));
        }

        [Test]
        public async Task DeleteProducts_Success()
        {
            var dto = new SoftDeleteProductDto
            {
                ProductId = 1,
                Reason = "Low stock"
            };

            await productRepository.SoftDeleteProduct(dto);

            var updatedProduct = await _db.Products.FirstAsync(x => x.ProductId == dto.ProductId);
            Assert.That(updatedProduct.IsActive, Is.EqualTo(false));
        }

        //RestoreProduct
        [Test]
        public void RestoreProduct_Failure()
        {
            int id=10;
            Assert.ThrowsAsync<NotFoundException>(async()=>await productRepository.RestoreProduct(id));
        }


        [Test]
        public async Task RestoreProduct_Success()
        {
            int id = 4;
            await productRepository.RestoreProduct(id);
            var updatedProducts=await _db.Products.FirstAsync(x=>x.ProductId == id);    
            Assert.That(updatedProducts.IsActive, Is.EqualTo(true));
        }

        //AddImage
        [Test]
        public async Task AddImage_Success()
        {
            int productId = 3;

            var images = new List<IFormFile>
            {
                CreateFakeFile("test1.png"),
                CreateFakeFile("test2.png")
            };

            await productRepository.AddImage(productId, images);

            Assert.Pass();
        }

        [Test]
        public void AddImage_InavlidId()
        {

            int productId = 10;

            var images = new List<IFormFile>
            {
                CreateFakeFile("test1.png"),
                CreateFakeFile("test2.png")
            };

            Assert.ThrowsAsync<NotFoundException>(async () => await productRepository.AddImage(productId, images));
        }

        [Test]
        public void AddImage_NoImages_ThrowException()
        {
            int productId = 1;

            Assert.ThrowsAsync<ArgumentNullException>(async () => await productRepository.AddImage(productId, null));
        }


        [Test]
        public void AddImage_AnyImages_ThrowException()
        {
            int productId = 1;
            var images = new List<IFormFile>
            {

            };

            Assert.ThrowsAsync<ArgumentException>(async () => await productRepository.AddImage(productId, images));
        }


        [Test]
        public void UpdateImage_Fails_InvalidId()
        {
            int id = 10;
            var dto = new ProductResponse
            {
                ProductName = "Product1",
                CategoryId = 1,
                Brand = "Tata",
                Type = "Panel",
                CapacityValue = 500,
                Capacity = "500W",
                Price = 15000,
                Warranty_Years = 5,
                Stock = 10,
                Description = "High efficiency solar panel",


            };

            Assert.ThrowsAsync<NotFoundException>(async () => await productRepository.UpdateProduct(id, dto));

        }

        [Test]
        public void UpdateProduct_SameProductName_Exception()
        {
            int id = 1;
            var dto = new ProductResponse
            {
                ProductName = "Solar Panel 300W",
                CategoryId = 1,
                Brand = "Tata",
                Type = "Panel",
                CapacityValue = 500,
                Capacity = "500W",
                Price = 15000,
                Warranty_Years = 5,
                Stock = 10,
                Description = "High efficiency solar panel",


            };

            Assert.ThrowsAsync<DataExistsException>(async () => await productRepository.UpdateProduct(id, dto));

        }

        [Test]
        public async Task UpdateProduct_Success()
        {
            int id = 1;
            var dto = new ProductResponse
            {
                ProductName = "Solar Panel 900W",
                CategoryId = 1,
                Brand = "Tata",
                Type = "Panel",
                CapacityValue = 500,
                Capacity = "500W",
                Price = 10000,
                Warranty_Years = 5,
                Stock = 10,
                Description = "High efficiency solar panel",


            };

            await productRepository.UpdateProduct(id, dto);

            var updatedProduct = await _db.Products.FirstAsync(x => x.ProductId == id);
            Assert.That(updatedProduct.Price,Is.EqualTo(10000));
        }


        //DeleteProductById
        [Test]
        public void DeleteProductById_FailureId()
        {
            Assert.ThrowsAsync<NotFoundException> (async () => await productRepository.DeleteProductById(10));
        }

        [Test]
        public async Task DeleteProductById_Success()
        {
            int id = 1;
            await productRepository.DeleteProductById(id);
            var updatedProduct = await _db.Products.FirstAsync(x => x.ProductId == id);
            Assert.That(updatedProduct.IsActive, Is.EqualTo(false));
        }


        //UpdateImage
        [Test]
        public void UpdateProductImage_Failure()
        {
            int imageId = 101;
            var image = CreateFakeFile("test.png");

            Assert.ThrowsAsync<NotFoundException>(async () => await productRepository.UpdateProductImage(imageId, image));
        }

        [Test]
        public async Task UpdateProductImage_Success()
        {

            int imageId = 1;
            var image = CreateFakeFile("test.png");
            await productRepository.UpdateProductImage(imageId,image);

            
            
            Assert.Pass();
        }


        //GetImage
        [Test]
        public void GetImage_Falure_Id()
        {
            var productId = 10;
            Assert.ThrowsAsync<NotFoundException>(async () => await productRepository.GetIamges(productId));
        }

        [Test]
        public async Task GetImage_Success()
        {
            int id = 1;
            var result=await productRepository.GetIamges(id);
            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(result);
                Assert.That(result.First().ImageId, Is.EqualTo(1));
            });
        }

        //DeleteImage
        [Test]
        public void DeleteImage_Failure_Id()
        {
            int imageId = 10;
            Assert.ThrowsAsync<NotFoundException>(async()=>await productRepository.DeleteImage(imageId));
        }

        [Test]
        public async Task DeleteImage_Failure_Success()
        {
            int imageId = 1;
            await productRepository.DeleteImage(imageId);

            var updatedImage=await _db.ProductImages.FindAsync(imageId);
            if(updatedImage != null)
            {
                Assert.That(updatedImage.IsActive, Is.EqualTo(false));
            }
        }
    }
}
