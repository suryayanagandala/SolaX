using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.Entities;
using DbAccessaLayer.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace SolarProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolarPerfomanceController : ControllerBase
    {
        private readonly SolarPerformanceService solarPerformance;
        private ILogger<SolarPerfomanceController> logger;
        public SolarPerfomanceController(IUnitOfWork unitOfWork,ILogger<SolarPerfomanceController> logger)
        {
         solarPerformance=new SolarPerformanceService(unitOfWork);  
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result=await solarPerformance.GetUsers();
            return Ok(result);
        }

        [HttpGet("GetPerformance")]
        public async Task<GenericResponse<SolarPerformance>> GetPerformace(int? panelId=null)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (userId != null)
                {
                    var result= await solarPerformance.GetPerofrmance(userId, panelId);
                    logger.LogInformation($"Fetching the performance data {userId}");
                    return new GenericResponse<SolarPerformance> { Status=true,Data= result};
                }

                return new GenericResponse<SolarPerformance> { Status = false, Error = MessageHelper.Get("User", "UserId") };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new GenericResponse<SolarPerformance> { Status = false, Error = ex.Message };
            }
        }

        [HttpGet("GetMonthlyUpdates")]
        public async Task<GenericResponse<List<Object>>> GetMonthlyUpdates(int? panelId=null)
        {
            try
            {
                var userId=User.FindFirst("UserId")?.Value;
                if (userId != null)
                {
                    var data = await solarPerformance.GetMonthlyUpdates(userId, panelId);
                    logger.LogInformation($"{nameof(GetMonthlyUpdates)}");
                    return new GenericResponse<List<Object>> { Status = true, Data = data };
                }
                return new GenericResponse<List<Object>> { Status = false, Error = MessageHelper.Get("User","UserId") };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new GenericResponse<List<object>> { Status = false, Error = ex.Message };
            }
        }

        [HttpGet("GetDashboardSummary")]
        public async Task<GenericResponse<object>> GetSummaryDashboard(int? panelId=null)
        {
            try
            {
                var userId= User.FindFirst("UserId")?.Value;
                if (userId != null)
                {
                    var data = await solarPerformance.GetCustomerDashboard(userId,panelId);
                    logger.LogInformation($"{nameof(GetSummaryDashboard)}");
                    return new GenericResponse<object> { Status = true, Data = data };
                }
                return new GenericResponse<object> { Status = false, Error = MessageHelper.Get("User", "UserId") };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new GenericResponse<object> { Status = false, Error = ex.Message };
            }
        }

        [HttpGet("AddToPanels")]
        public async Task<GenericResponse<string>> AddToPanles()
        {
            try
            {
                await solarPerformance.AddToPanle();
                logger.LogInformation($"{nameof(AddToPanles)} at {TimeUtility.NowIST()}");
                return new GenericResponse<string> { Status = true, Data = "Succesfully added" };
            }
            catch (DbUpdateException ex)
            {
                var errormEssages = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                logger.LogError(errormEssages);
                return new GenericResponse<string> { Status = false, Error = errormEssages };
            }
        }

        [HttpGet("GetActivatedSolarPanles")]
        public async Task<GenericResponse<List<object>>> GetActivatedObjects()
        {
            try
            {
                var userId=User.FindFirst("UserId")?.Value;
                if (userId == null) return new GenericResponse<List<object>> { Status = false, Error = MessageHelper.Get("User", "UserId") };
                var data=await solarPerformance.GetActivatedSolarPanles(userId);
                logger.LogInformation($"{userId}:{data}");
                return new GenericResponse<List<object>> {Status=true, Data = data };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new GenericResponse<List<object>> { Status = false, Error = ex.Message };
            }
        }

        [HttpGet("GetPanelDetails/{panelId}")]
        public async Task<GenericResponse<object>> GetPanelDetails([FromRoute] int panelId)
        {
            try
            {
                var data=await solarPerformance.GetPanelDetails(panelId);
                logger.LogInformation($"Panel details: {data}");
                return new GenericResponse<object> { Status = true, Data = data };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new GenericResponse<object> { Status = false, Error = ex.Message };
            }
        }


    }
}
