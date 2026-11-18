using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.DTO_S;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Diagnostics;

namespace SolarProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class HomeController : ControllerBase
    {
        private readonly HomeService homeService;
        private ILogger<HomeController> logger;
        public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger)
        {
            homeService = new HomeService(unitOfWork);
            this.logger = logger;
        }

        [HttpGet("GetHomeCategories")]
       public async Task<OutputResponse> GetHomeCategory()
        {
            try
            {
               var data= await homeService.GetHomeCategories();
                logger.LogInformation("fetching home categories on main page");
                return new OutputResponse { Status = true, Data = data };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse {Status=false, Error = ex.Message };
            }
        }

        [HttpGet("GetHomePackage")]
        public async Task<OutputResponse> GetHomeContents()
        {
            try
            {
                var data = await homeService.GetHomePackage();
                logger.LogInformation($"{data} fetching the data");
                return new OutputResponse {Status = true, Data = data };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse   { Status=false, Error = ex.Message };
            }
        }

        [HttpGet("GetProductsBySlug")]
        public async Task<OutputResponse>GetProducts([FromQuery]string slug)
        {
            try
            {
                var data = await homeService.GetCategoriesWiseBySlug(slug);
                logger.LogInformation($" based on {slug} getting the result");
                return new OutputResponse { Status = true, Data = data };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }


            
        }

        [HttpGet("GetProductDetails/{productId}")]
        public async Task<OutputResponse>ViewProductDetails(int productId)
        {
            try
            {
                var data = await homeService.ViewProductDetails(productId);
                logger.LogInformation($"getting the products base on {productId}");
                return new OutputResponse { Status = true, Data = data };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }


        [HttpGet("GetProductAnalytics/{productId}")]
        public async Task<OutputResponse> GetProductAnalytics([FromRoute] int productId)
        {
            try
            {
                var data=await homeService.GetAnalyticsOfProduct(productId);
                logger.LogInformation("Viewing analytics of dashboard on"+DateTime.Now);
                return new OutputResponse { Status = true, Data = data };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status=false, Error = ex.Message };
            }
        }

    }
}
