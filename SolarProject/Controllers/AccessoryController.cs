using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.DTO_S;
using DbAccessaLayer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SolarProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessoryController : ControllerBase
    {
        private readonly AccesoryService accesoryService;

        private readonly ILogger<AccessoryController> logger;
        public AccessoryController(IUnitOfWork unitOfWork,ILogger<AccessoryController> logger)
        {
            accesoryService = new AccesoryService(unitOfWork);
            this.logger = logger;
        }

      


        [HttpGet]
        [Route("GetRecommendations")]
        public async Task<GenericResponse<RecommendationResponseDto>> GetFullRecommendations([FromQuery] int productId)
        {
            try
            {
                var data = await accesoryService.GetFullRecommendationsAsync(productId);
                logger.LogInformation($"Get AiRecommendations of{productId}");
                return new GenericResponse<RecommendationResponseDto> { Status = true, Data = data };

            }
            catch (Exception ex)

            {
                var errorMessages = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                logger.LogError(errorMessages);
                return new GenericResponse<RecommendationResponseDto> { Status = false, Error = errorMessages };
            }
        }

        [HttpGet("Why-this")]
        public async Task<OutputResponse> GetWhyThisExplanations(int mainProductId, int recommendedProductId)
        {
            try
            {
                var data=await accesoryService.GetWhyThis(mainProductId, recommendedProductId);
                logger.LogInformation(data);
                return new OutputResponse { Status = true, Data = data };
            }
            catch(Exception ex)
            {
                logger.LogError($"{ex.Message}", ex);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }


        [HttpGet("ComapareProducts")]
        public async Task<OutputResponse> CompareProducts(int product1Id, int product2Id)
        {
            try
            {
                var data=await accesoryService.CompareProducts(product1Id, product2Id);
                logger.LogInformation(data);
                return new OutputResponse { Status = true, Data = data };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status= false, Error = ex.Message };
            }
        }


        [HttpGet("GetBestComboProducts")]
        public async Task<OutputResponse> GetBestCombo(int productId)
        {
            try
            {
                var data=await accesoryService.GetBestCombo(productId);
                //logger.LogInformation(data.Battery.ToString());
                return new OutputResponse { Status = true,Data= data };
            }
            catch(Exception ex)
            {
                logger.LogError($"{ex.Message}");
                return new OutputResponse { Status=false,Error = ex.Message };
            }
        }

        [HttpGet("OptimizeBudgetProducts")]
        public async Task<OutputResponse> GetBudgetProducts(int productId,decimal budget)
        {
            try
            {
                var data=await accesoryService.OptimizeBudgetProducts(productId,budget);
                logger.LogInformation(data);
                return new OutputResponse
                {
                    Status = true,
                    Data = data
                };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status=false,Error = ex.Message };
            }
        }


        

        [Authorize]
        [HttpGet("PredictionRequests")]
        public async Task<OutputResponse> ResponseAsync()
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new OutputResponse { Status = false, Error = MessageHelper.Get("User", "UserId") };
                }
                var data = await accesoryService.GetWhetherForecast(userId);
                return new OutputResponse { Status = true,Data=data};
            }
            catch (Exception ex)
            {
                logger.LogError($"{ex.Message}");
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }


        [HttpGet("GetDailyPredictions")]
        public  async  Task<OutputResponse> GetDailyPrediction()
        {
            try
            {
                var userId=User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new OutputResponse { Status = false, Error = MessageHelper.Get("User", "UserId") };
                }
                userId = userId.ToString();
                var dailyPredictionData=await accesoryService.GetDailyForecasts(userId);
                return new OutputResponse { Status = true, Data =dailyPredictionData };

            }
            catch(Exception ex)
            {
                return new OutputResponse { Status=false,Error = ex.Message };
            }
        }

        [HttpPost("CalculateSizing")]
        public async Task<OutputResponse> CalculateSizing([FromBody] SolarSizingRequest request)
        {
            try
            {
                if (request.MonthlyBill == null && request.MonthlyUnits == null)
                    return new OutputResponse { Status = false, Error = "plase enter atleast one " };
                var data=await accesoryService.CalculateSizing(request);
                return new OutputResponse { Status = true, Data = data };
            }
            catch(Exception ex)
            {
                var errorMessages = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new OutputResponse { Status = false, Error = errorMessages };
            }
        }



    }
}
