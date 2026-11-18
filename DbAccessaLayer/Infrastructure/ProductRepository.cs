using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using CoreModels.Entities;
using DbAccessaLayer.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
//using MimeKit.Encodings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure
{
    public class ProductRepository : IProductsRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductRepository(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<ProductDto>> GetAllPrducts()
        {
            try
            {
                //await AddProducts();
                return await _db.Products.AsNoTracking().Select(x => new ProductDto
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Price = x.Price,
                    Type = x.Type,
                    CapacityValue = x.CapacityValue,
                    Stock = x.Stock,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedReason = x.DeletedReason,
                    Description = x.Description,
                    CategoryId =(int) x.CategoryId,
                    Brand=x.Brand,
                    Capacity=x.Capacity,
                    Category = x.Category == null ? null : new CategoryDto
                    {
                        CategoryId = x.Category.CategoryId,
                        Name = x.Category.CategoryName,
                        Slug = x.Category.Slug,
                        Description = x.Category.Description,
                        IsActive = x.Category.IsActive
                    },
                    ProductImages = x.ProductImages
                    .Select(img => new ProductImageResponseDto
                    {
                        ImageId = img.ImageId,
                        ImageUrl = img.ImageUrl,
                        IsActive = img.IsActive,
                    })
                    .ToList()

                }).ToListAsync();

                
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    throw new Exception(ex.InnerException.Message);
                }
                else
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        //AddProduct
        public async Task AddProduct(ProductResponse dto)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto), "Please fill all the details");
                }

                var existData = await _db.Categories.FirstOrDefaultAsync(x => x.CategoryId == dto.CategoryId && x.IsActive);

                if (existData == null)
                {
                    throw new NotFoundException("Invalid category Id or category is not active mode");
                }


                var productDetails = new Products
                {
                    ProductName = dto.ProductName,
                    Brand = dto.Brand,
                    Type = dto.Type.ToString(),
                    CapacityValue = dto.CapacityValue,
                    Capacity = dto.Capacity,
                    Price = dto.Price,
                    Warranty_Years = dto.Warranty_Years,
                    Stock = dto.Stock,
                    Description = dto.Description,
                    CreatedAt = TimeUtility.NowIST(),
                    CategoryId = dto.CategoryId,
                    IsActive=true,
                };
                _db.Products.Add(productDetails);
                await _db.SaveChangesAsync();
              
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<ProductDto> GetProductById(int productId)
        {
            
                var checkExistId = await _db.Products.FirstOrDefaultAsync(x=>x.ProductId==productId);
                if (checkExistId==null)
                {
                    throw new NotFoundException(MessageHelper.Get("Products", "NotFound"));
                }
                else
                {
                    return new ProductDto
                    {
                        ProductName = checkExistId.ProductName,
                        Brand = checkExistId.Brand,
                        Type = checkExistId.Type,
                        CapacityValue = checkExistId.CapacityValue,
                        Capacity = checkExistId.Capacity,
                        Price = checkExistId.Price,
                        Description = checkExistId.Description,
                        Stock = checkExistId.Stock,
                        Warranty_Years = checkExistId.Warranty_Years,
                        CategoryId= checkExistId.CategoryId,
                    };
                } 
            
            
        }


        //soft delete
        public async Task SoftDeleteProduct(SoftDeleteProductDto dto)
        {
            var product = await _db.Products.FirstOrDefaultAsync(x => x.ProductId == dto.ProductId && x.IsActive);

            if (product == null)
                throw new NotFoundException(MessageHelper.Get("Products", "NotFound"));

            product.IsActive = false;
            product.DeletedReason = dto.Reason;
            product.DeletedAt = DateTimeOffset.UtcNow;

            await _db.SaveChangesAsync();
        }

        //restore

        public async Task RestoreProduct(int productId)
        {
            var product = await _db.Products.FirstOrDefaultAsync(x => x.ProductId == productId && !x.IsActive);

            if (product == null)
                throw new NotFoundException(MessageHelper.Get("Products", "NotFound"));

            product.IsActive = true;
            product.DeletedReason = null;
            product.DeletedAt = null;
            product.UpdatedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync();
        }

        //AddProductIamges
        public async Task AddImage(int productId, List<IFormFile> images)
        {
            try
            {
                var productExists = await _db.Products.AnyAsync(x => x.ProductId == productId &&x.IsActive);
                if (!productExists)
                {
                    throw new NotFoundException(MessageHelper.Get("Products", "NotFound"));

                }
                if (images == null)
                    throw new ArgumentNullException(nameof(images));

                if (!images.Any())
                    throw new ArgumentException("Please upload at least one image", nameof(images));


                foreach (var image in images)
                {
                    var path = await ImageHelper.SaveProductIamges(image, this.webHostEnvironment);

                    var productImage = new ProductImage
                    {
                        ProductId = productId,
                        ImageUrl = path,
                        CreatedAt=TimeUtility.NowIST()
                    };

                    await _db.ProductImages.AddAsync(productImage);
                    
                }
               await  _db.SaveChangesAsync();
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task UpdateProduct(int productId, ProductResponse dto)
        {
                var existProduct = await _db.Products.FindAsync(productId);
                if (existProduct == null)
                {
                    throw new NotFoundException(MessageHelper.Get("Products", "NotFound"));
                }
                var exists = await _db.Products.AnyAsync(x => x.ProductName == dto.ProductName && x.ProductId != productId);

                if (exists)
                {
                    throw new DataExistsException(MessageHelper.Get("Products", "Match"));
                }
               

                else
                {
                    existProduct.Capacity = dto.Capacity;
                    existProduct.UpdatedAt = TimeUtility.NowIST();
                    existProduct.Price = dto.Price;
                    existProduct.ProductName = dto.ProductName;
                    existProduct.Type = dto.Type.ToString();
                    existProduct.Brand = dto.Brand;
                    existProduct.Stock= dto.Stock;
                    existProduct.Warranty_Years = dto.Warranty_Years;
                    existProduct.CapacityValue = dto.CapacityValue;
                    existProduct.IsActive = true;
                    existProduct.CategoryId=dto.CategoryId;

                    await _db.SaveChangesAsync();
                }
            
            
        }

        public async Task DeleteProductById(int productId)
        {
            
                var existProductId = await _db.Products.FirstOrDefaultAsync(x=>x.ProductId==productId);
                if (existProductId == null)
                {
                    throw new NotFoundException(MessageHelper.Get("Products", "NotFound"));
                }
                else
                {
                    existProductId.IsActive = false;
                    await _db.SaveChangesAsync();
                }
           
        }

        public async Task UpdateProductImage(int imageId, IFormFile image)
        {
            

                var existingId = await _db.ProductImages.FirstOrDefaultAsync(x => x.ImageId == imageId &&x.IsActive);
                if (existingId==null)
                {
                    throw new NotFoundException(MessageHelper.Get("Products", "NoImage"));
                }
                else
                {
                    string newPath = await ImageHelper.UpdateProductImages(image, webHostEnvironment, existingId.ImageUrl);
                    existingId.ImageUrl = newPath;
                    existingId.UpdatedAt = TimeUtility.NowIST();
                    existingId.IsActive = true;
                    await _db.SaveChangesAsync();
                }
           
        }
        
        public async Task<IEnumerable<ProductImageResponseDto>> GetIamges(int productId)
        {
           
                var existProductId = await _db.Products.Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.ProductId == productId&&x.IsActive);
                if (existProductId == null)
                {
                    throw new NotFoundException(MessageHelper.Get("Products", "NotFound"));

                }
                else
                {
                    return existProductId.ProductImages.Where(x => x.IsActive).Select(x => new ProductImageResponseDto
                    {
                        ImageId=x.ImageId,
                        ImageUrl=x.ImageUrl,
                        CreatedAt=x.CreatedAt,
                        UpdatedAt=x.UpdatedAt
                    }).ToList();
                }
            
        }

        public async Task DeleteImage(int imageId)
        {
            try
            {
                var existDataId = await _db.ProductImages.FirstOrDefaultAsync(x => x.ImageId == imageId);
                if (existDataId == null)
                {
                    throw new NotFoundException(MessageHelper.Get("Products","NoImage"));
                }
                else
                {
                    if (!string.IsNullOrEmpty(existDataId.ImageUrl))
                    {
                        string existingPath = Path.Combine(webHostEnvironment.WebRootPath, existDataId.ImageUrl.TrimStart('/'));
                        if (File.Exists(existingPath))
                        {
                            File.Delete(existingPath);
                        }
                    }
                    existDataId.IsActive = false;
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


       public async Task AddProducts()
        {
            try
            {
                var accessories = new List<Products>
                {
   
                    new Products { ProductName="Polycab Solar DC Cable 4mm", Brand="Polycab", Type="Cable", CapacityValue=4, Capacity="4mm", Price=50, Warranty_Years=1, Stock=200, Description="High quality DC solar cable", CategoryId=35, CreatedAt=TimeUtility.NowIST() },
                    new Products { ProductName="Havells Solar Cable 6mm", Brand="Havells", Type="Cable", CapacityValue=6, Capacity="6mm", Price=70, Warranty_Years=1, Stock=180, Description="Durable solar cable for installations", CategoryId=35, CreatedAt=TimeUtility.NowIST() },
                    new Products { ProductName="Finolex DC Wire 10mm", Brand="Finolex", Type="Cable", CapacityValue=10, Capacity="10mm", Price=100, Warranty_Years=1, Stock=150, Description="Heavy duty solar wiring", CategoryId=35, CreatedAt=TimeUtility.NowIST() },
                    new Products { ProductName="KEI Solar Cable 2.5mm", Brand="KEI", Type="Cable", CapacityValue=2, Capacity="2.5mm", Price=40, Warranty_Years=1, Stock=220, Description="Flexible solar cable", CategoryId=35, CreatedAt=TimeUtility.NowIST() },

                    new Products { ProductName="Aluminium Solar Mount Kit", Brand="Generic", Type="Mounting", CapacityValue=1, Capacity="Standard", Price=5000, Warranty_Years=5, Stock=50, Description="Roof mounting structure", CategoryId=36, CreatedAt=TimeUtility.NowIST() },
                    new Products { ProductName="Galvanized Iron Mount Frame", Brand="Generic", Type="Mounting", CapacityValue=1, Capacity="Heavy Duty", Price=7000, Warranty_Years=5, Stock=40, Description="Strong mounting frame", CategoryId=36, CreatedAt=TimeUtility.NowIST() },
                    new Products { ProductName="Adjustable Solar Mount", Brand="Generic", Type="Mounting", CapacityValue=1, Capacity="Adjustable", Price=6500, Warranty_Years=5, Stock=35, Description="Tilt adjustable mounting", CategoryId=36, CreatedAt=TimeUtility.NowIST() },

                    new Products { ProductName="MC4 Connector Pair", Brand="MultiContact", Type="Connector", CapacityValue=1, Capacity="Pair", Price=300, Warranty_Years=1, Stock=300, Description="Standard MC4 connector", CategoryId=37, CreatedAt=TimeUtility.NowIST() },
                    new Products { ProductName="MC4 Y Connector", Brand="Generic", Type="Connector", CapacityValue=1, Capacity="Y Type", Price=400, Warranty_Years=1, Stock=250, Description="Parallel connection connector", CategoryId=37, CreatedAt=TimeUtility.NowIST() },
                    new Products { ProductName="Solar Junction Box", Brand="Generic", Type="Connector", CapacityValue=1, Capacity="Standard", Price=600, Warranty_Years=1, Stock=150, Description="Panel junction box", CategoryId=37, CreatedAt=TimeUtility.NowIST() },


                    new Products { ProductName="Solar Monitoring WiFi Dongle", Brand="Growatt", Type="Monitoring", CapacityValue=1, Capacity="WiFi", Price=3500, Warranty_Years=2, Stock=60, Description="Monitor system via mobile", CategoryId=38, CreatedAt=TimeUtility.NowIST() },
                    new Products { ProductName="Energy Meter Digital", Brand="L&T", Type="Monitoring", CapacityValue=1, Capacity="Digital", Price=2500, Warranty_Years=2, Stock=70, Description="Track energy usage", CategoryId=38, CreatedAt=TimeUtility.NowIST() },
                    new Products { ProductName="Solar Data Logger", Brand="Generic", Type="Monitoring", CapacityValue=1, Capacity="Logger", Price=5000, Warranty_Years=2, Stock=30, Description="Logs solar data", CategoryId=38, CreatedAt=TimeUtility.NowIST() },

 
                    new Products { ProductName="DC MCB 32A", Brand="Havells", Type="Protection", CapacityValue=32, Capacity="32A", Price=800, Warranty_Years=2, Stock=120, Description="Circuit protection device", CategoryId=39, CreatedAt=TimeUtility.NowIST() },
                    new Products { ProductName="AC MCB 16A", Brand="L&T", Type="Protection", CapacityValue=16, Capacity="16A", Price=500, Warranty_Years=2, Stock=140, Description="AC protection switch", CategoryId=39, CreatedAt=TimeUtility.NowIST() },
                    new Products { ProductName="Surge Protection Device", Brand="Schneider", Type="Protection", CapacityValue=1, Capacity="Standard", Price=2000, Warranty_Years=3, Stock=90, Description="Protects from voltage spikes", CategoryId=39, CreatedAt=TimeUtility.NowIST() },
                    new Products { ProductName="Earthing Kit", Brand="Generic", Type="Protection", CapacityValue=1, Capacity="Complete Kit", Price=1500, Warranty_Years=3, Stock=100, Description="Complete grounding solution", CategoryId=39, CreatedAt=TimeUtility.NowIST() },


                
                };

                foreach (var p in accessories)
                {
                    await _db.Products.AddAsync(p);

                }
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
