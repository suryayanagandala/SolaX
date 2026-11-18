using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductReviewRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<AddProductReviewDto> AddProductReview(string userId, AddProductReviewDto dto)
        {
            try
            {
                var existUser=await _db.Users.FindAsync(userId);
                if (existUser == null)
                {
                    throw new Exception($"No UserId of {userId} macthes");
                }
                else
                {
                    bool alreadyReviewed = await _db.ProductReviews.AnyAsync(r => r.ProductId == dto.ProductId && r.UserId == userId);

                    if (alreadyReviewed)
                        throw new Exception("You already reviewed this product");
                    else
                    {
                        var review = new ProductReview
                        {
                            ProductId = dto.ProductId,
                            UserId = userId,
                            Rating = dto.Rating,
                            Comment = dto.Comment,
                        };

                        await _db.ProductReviews.AddAsync(review);
                        await _db.SaveChangesAsync();
                        await UpdateProductRating(dto.ProductId);

                        return new AddProductReviewDto
                        {
                            ProductId = review.ProductId,
                            Comment = review.Comment,
                            Rating = review.Rating
                        };
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteProductReview(string userId, int productId)
        {
            try
            {
                var existRecord = await _db.ProductReviews.FirstOrDefaultAsync(x=>x.UserId==userId&&x.ProductId==productId);
                if (existRecord!=null)
                {
                    existRecord.IsActive = false;
                    await _db.SaveChangesAsync();
                    await UpdateProductRating(productId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ProductReviewResponseDto>> GetAllReviewByPrduct(int productId)
        {
            try
            {
               return await _db.ProductReviews.Where(x=>x.ProductId==productId&&x.IsActive).OrderByDescending(x=>x.CreatedAt)
                    .Select(x=> new ProductReviewResponseDto
                    {
                        UserId=x.UserId,
                        Comment=x.Comment,
                        Rating=x.Rating,
                        CreatedAt=x.CreatedAt
                    }).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AddProductReviewDto> UpdateProductReview(string userId,int productId, AddProductReviewDto dto)
        {
            try
            {
                var existReview = await _db.ProductReviews.FirstOrDefaultAsync(x => x.UserId == userId
                && x.ProductId == productId && x.IsActive);

                if (existReview == null)
                {
                    throw new Exception($"Review not found");
                }
                else
                {
                    existReview.Rating = dto.Rating;
                    existReview.Comment = dto.Comment;
                    existReview.IsActive = true;
                    existReview.UpdatedAt = TimeUtility.NowIST();

                    await _db.SaveChangesAsync();
                    await UpdateProductRating(productId);

                    return new AddProductReviewDto
                    {

                        Rating=existReview.Rating,
                        Comment=existReview.Comment,
                        ProductId=existReview.ProductId
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async  Task UpdateProductRating(int productId)
        {
            try
            {
                var reviews=await _db.ProductReviews.Where(x=>x.ProductId==productId&& x.IsActive).ToListAsync();

                var product = await _db.Products.FindAsync(productId);

                if(product != null&& reviews.Any())
                {
                    product.RatingCount = reviews.Count();
                    product.AverageRating = Math.Round(reviews.Average(x => x.Rating), 2);
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
