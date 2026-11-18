using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using DbAccessaLayer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;

namespace SolarProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductReviewController : ControllerBase
    {
        private readonly ProductReviewsService productReview;

        public ProductReviewController(IUnitOfWork unitOfWork)
        {
            productReview = new ProductReviewsService(unitOfWork);
        }

        [HttpGet("GetAllReviws/{productId}")]
        public async Task<GenericResponse<IEnumerable<ProductReviewResponseDto>>> GetAllReviewByPrduct(int productId)
        {
            try
            {
                var data=await productReview.GetAllReviewByPrduct(productId);
                return new GenericResponse<IEnumerable<ProductReviewResponseDto>> { Status=true,Data=data };
            }
            catch (Exception ex)
            {
                return new GenericResponse<IEnumerable<ProductReviewResponseDto>> {Status=false,Error=ex.Message};
            }
        }

        [HttpPost("AddProductReview/{userId}")]
        public async Task<GenericResponse<AddProductReviewDto>> AddProductReview( [FromBody]AddProductReviewDto dto)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;

                if (string.IsNullOrEmpty(userId))
                    return new GenericResponse<AddProductReviewDto> { Status = false, Error = "Invalid UserId" };
                var data= await productReview.AddProductReview(userId, dto);

                return new GenericResponse<AddProductReviewDto> { Status = true, Data = data };

            }
            catch (Exception ex)
            {
                return new GenericResponse<AddProductReviewDto> { Status=false, Error=ex.Message};
            }
        }

        [HttpPatch("ProductReview/{productId}")]
        public async Task<GenericResponse<AddProductReviewDto>> UpdateProductReview( int productId, [FromBody]AddProductReviewDto dto)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;

                if (string.IsNullOrEmpty(userId))
                    return new GenericResponse<AddProductReviewDto> { Status = false, Error = "Invalid UserId" };
                var data=await productReview.UpdateProductReview(userId,productId,dto);

                return new GenericResponse<AddProductReviewDto> { Status = true, Data = data };
            }
            catch(Exception ex)
            {
                return new GenericResponse<AddProductReviewDto> { Status = false, Error = ex.Message };
            }
        }

        [HttpDelete("DeleteReview/{productId}")]
        public async Task<GenericResponse<string>> DeleteProductReview( int productId)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;

                if (string.IsNullOrEmpty(userId))
                    return new GenericResponse<string> { Status = false, Error = "Invalid UserId" };
                 await productReview.DeleteProductReview(userId, productId);
                return new GenericResponse<string> { Status = true, Data = MessageHelper.Get("ProductReview", "Deleted") };

            }
            catch (Exception ex)
            {
                return new GenericResponse<string> { Status = false, Error = ex.Message };
            }
        }
    }
}
