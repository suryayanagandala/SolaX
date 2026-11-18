using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using CoreModels.Enums;
using DbAccessaLayer.Helpers;
using Microsoft.EntityFrameworkCore;
//using MimeKit.Encodings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;
        public HomeRepository(ApplicationDbContext db)
        {
            _db=db;
        }

        public async Task<List<CategoryDto>> GetHomeCategories()
        {
            var categories = await _db.Categories
                .Where(x => x.ParentCategoryId == null && x.IsActive)
                .Include(x => x.SubCategories)
                .ToListAsync();

            return categories.Select(x => new CategoryDto
            {
                CategoryId = x.CategoryId,
                Name = x.CategoryName,
                Description = x.Description,
                Slug = x.Slug,
                ImageUrl=x.ImageUrl,
                Children = x.SubCategories
               
                    .Where(c => c.IsActive)
                    .Select(c => new CategoryDto
                    {
                        CategoryId = c.CategoryId,
                        Name = c.CategoryName,
                        Description = c.Description,
                        Slug = c.Slug,
                        ParentId = x.ParentCategoryId,
                    }).ToList()
            }).ToList();
        }

        public async Task<List<CategoryDto>> GetHomePackage()
        {
            try
            {
                var categories = await _db.Categories.Include(x => x.SubCategories.Where(sc => sc.IsActive)).ThenInclude(x => x.Products.Where(pc => pc.IsActive))
                    .ThenInclude(x => x.ProductImages.Where(pi => pi.IsActive)).Include(x => x.Products.Where(pc => pc.IsActive))
                    .ThenInclude(i => i.ProductImages.Where(pi => pi.IsActive)).Where(x=>x.IsActive&&x.ParentCategoryId==null).ToListAsync();
                if (!categories.Any())
                {
                    throw new NotFoundException(MessageHelper.Get("Categories", "NotFoundCategories"));
                }
                else
                {
                    var result = categories.Select(c => new CategoryDto
                    {
                        CategoryId = c.CategoryId,
                        Name = c.CategoryName,
                        Description = c.Description,
                        Slug = c.Slug,
                        IsActive=c.IsActive,
                        ParentId=c.ParentCategoryId,

                        // CHILD CATEGORIES
                        Children = c.SubCategories.Select(x => new CategoryDto
                        {
                            CategoryId = x.CategoryId,
                            Name = x.CategoryName,
                            Description = x.Description,
                            Slug = x.Slug,
                            IsActive=x.IsActive,
                            ParentId=x.ParentCategoryId,

                            // CHILD PRODUCTS
                            ProductsDto = x.Products.Select(p => new ProductDto
                            {
                                ProductId = p.ProductId,
                                ProductName = p.ProductName,
                                Price = p.Price,
                                Brand = p.Brand,
                                Type = p.Type,
                                Capacity = p.Capacity,
                                CapacityValue = p.CapacityValue,
                                Description = p.Description,
                                Stock = p.Stock,
                                CategoryId = p.CategoryId,
                                IsActive=p.IsActive,
                                ProductImages = p.ProductImages.Select(i => new ProductImageResponseDto
                                {
                                    ImageId = i.ImageId,
                                    ImageUrl = i.ImageUrl,
                                    IsActive=i.IsActive,
                                }).ToList()
                            }).ToList()
                        }).ToList(),

                        ProductsDto = c.Products.Select(p => new ProductDto
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            Price = p.Price,
                            Brand = p.Brand,
                            Type = p.Type,
                            Capacity = p.Capacity,
                            CapacityValue = p.CapacityValue,
                            Description = p.Description,
                            Stock = p.Stock,
                            CategoryId = p.CategoryId,
                            ProductImages = p.ProductImages.Select(i => new ProductImageResponseDto
                            {
                                ImageId = i.ImageId,
                                ImageUrl = i.ImageUrl
                            }).ToList()
                        }).ToList()

                    }).ToList();
                    return result;
                }

                }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<CategoryDto> GetProductBySlug(string slug)
        {
               
                var category = await _db.Categories.AsNoTracking().Include(c => c.SubCategories.Where(sc => sc.IsActive))
                        .ThenInclude(sc => sc.Products.Where(p => p.IsActive)).ThenInclude(p => p.ProductImages.Where(pi => pi.IsActive))
                        .Include(c => c.Products.Where(p => p.IsActive))
                        .ThenInclude(p => p.ProductImages.Where(pi => pi.IsActive))
                        .FirstOrDefaultAsync(c => c.Slug == slug  && c.IsActive);

                if (category == null)
                    throw new NotFoundException(MessageHelper.Get("Categories", "slugNotFound"));

              
                var categoryDto = new CategoryDto
                {
                    CategoryId = category.CategoryId,
                    Name = category.CategoryName,
                    Slug = category.Slug,
                    Description = category.Description,

                    
                    Children = category.SubCategories.Select(sc => new CategoryDto
                    {
                        CategoryId = sc.CategoryId,
                        Name = sc.CategoryName,
                        Slug = sc.Slug,
                        Description = sc.Description,
                        ProductsDto = sc.Products.Select(p => new ProductDto
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            CategoryId=p.CategoryId,
                            Brand = p.Brand,
                            Type = p.Type,
                            Capacity = p.Capacity,
                            CapacityValue = p.CapacityValue,
                            Price = p.Price,
                            Stock = p.Stock,
                            Warranty_Years = p.Warranty_Years,
                            Description = p.Description,
                            IsActive = p.IsActive,
                            ProductImages = p.ProductImages.Select(pi => new ProductImageResponseDto
                            {
                                ImageId = pi.ImageId,
                                ImageUrl = pi.ImageUrl
                            }).ToList()
                        }).ToList()
                    }).ToList(),

                   
                    ProductsDto = !category.SubCategories.Any()
                        ? category.Products.Select(p => new ProductDto
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            Brand = p.Brand,
                            Type = p.Type,
                            Capacity = p.Capacity,
                            CapacityValue = p.CapacityValue,
                            Price = p.Price,
                            Stock = p.Stock,
                            Warranty_Years = p.Warranty_Years,
                            Description = p.Description,
                            IsActive = p.IsActive,
                            ProductImages = p.ProductImages.Select(pi => new ProductImageResponseDto
                            {
                                ImageId = pi.ImageId,
                                ImageUrl = pi.ImageUrl
                            }).ToList()
                        }).ToList()
                        : new List<ProductDto>()
                };

                return categoryDto;
            
            
        }

        public async Task<ProductDto> ViewProductDetails(int productId)
        {
            
                var product = await _db.Products.Include(r=>r.ProductReviews).Include(x => x.ProductImages).Include(x=>x.Category)
                    .FirstOrDefaultAsync(x => x.ProductId == productId && x.IsActive);
                if (product == null)
                {
                    throw new NotFoundException(MessageHelper.Get("Products","NotFound"));
                }
                else
                {
                    var result = new ProductDto
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        Price = product.Price,
                        Type = product.Type,
                        Brand = product.Brand,
                        CapacityValue = product.CapacityValue,
                        Description = product.Description,
                        Capacity=product.Capacity,
                        Stock=product.Stock,
                        AverageRating=product.AverageRating,
                        RatingCount=product.RatingCount,
                        CategoryName=product.Category.CategoryName,
                        CategoryId=product.Category.CategoryId,
                        Warranty_Years=product.Warranty_Years,
                        IsActive=product.IsActive,
                        ProductImages = product.ProductImages.Where(x=>x.IsActive).Select(x => new ProductImageResponseDto
                        {
                            ProductId = x.ProductId,
                            ImageId = x.ImageId,
                            ImageUrl = x.ImageUrl
                        }).ToList(),
                        productReviews=product.ProductReviews.Select(r=> new ProductReviewResponseDto
                        {
                            Comment=r.Comment,
                            Rating=r.Rating,
                            UserId=r.UserId
                        }).ToList()
                    };
                    return result;
                }
           
        }



        public async Task<ProductAnalyticsDto> GetProductAnalytics(int productId)
        {
            var quantity = await _db.OrderItems
                .Where(x => x.ProductId == productId && x.Order.Status == OrderStatus.DELIVERED)
                .SumAsync(x => (int?)x.Quantity) ?? 0;

            var totalPowerDouble = await _db.OrderItems
                .Where(x => x.ProductId == productId && x.Order.Status == OrderStatus.DELIVERED)
                .SelectMany(x => x.Order.User.solarPerformances)
                .SumAsync(sp => (double?)sp.Power) ?? 0;

            decimal totalPower = (decimal)totalPowerDouble;

            string unit;
            decimal convertedPower;

            if (totalPower >= 1_000_000)
            {
                convertedPower = totalPower / 1_000_000;
                unit = "MW";
            }
            else if (totalPower >= 1_000)
            {
                convertedPower = totalPower / 1_000;
                unit = "kW";
            }
            else
            {
                convertedPower = totalPower;
                unit = "W";
            }

            return new ProductAnalyticsDto
            {
                ProductId = productId,
                Quantity = quantity,
                PowerGenerated = Math.Round(convertedPower, 2),
                Unit = unit
            };
        }

    }
}
