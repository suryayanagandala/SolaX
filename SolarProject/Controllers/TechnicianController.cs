using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Enums;
using DbAccessaLayer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SolarProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Technician")]
    public class TechnicianController : ControllerBase
    {
        private readonly TechnicianService technicianService;
        private readonly ILogger<TechnicianController> logger;
        public TechnicianController(IUnitOfWork unitOfWork, ILogger<TechnicianController> logger)
        {
            technicianService = new TechnicianService(unitOfWork);
            this.logger = logger;
        }

        [HttpGet("GetTechnician/{userId}")]
        public async Task<GenericResponse<TechnicianResponse>> GetData()
        {
            try
            {
                var user=User.FindFirst("UserId")?.Value;
                if(user == null)
                {
                    return new GenericResponse<TechnicianResponse> { Status = false, Error = MessageHelper.Get("User", "UserId") };
                }

                var data=await technicianService.GetTechncianResponse(user);
                logger.LogInformation("Get techncian response");
                return new GenericResponse<TechnicianResponse> { Status=true,Data=data};
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new GenericResponse<TechnicianResponse> { Status=false, Error=ex.Message };
            }
        }

        [HttpGet("Dashboard")]
        public async Task<GenericResponse<TechnicianDashboardDto>> GetDashboard()
        {
            try
            {
                var role=User.FindFirst(ClaimTypes.Role)?.Value;
                if (role != RoleIds.Technician.ToString())
                {
                    return new GenericResponse<TechnicianDashboardDto> { Status = false, Error = MessageHelper.Get("Technician", "Invalid") };
                }
                var userId=User.FindFirst("UserId")?.Value;
                if (userId == null)
                {
                    logger.LogInformation("Get technician dashboard of"+userId );
                    return new GenericResponse<TechnicianDashboardDto> { Status = false, Error = MessageHelper.Get("User", "UserId") };
                }

                var data = await technicianService.GetDashboard(userId);
                return new GenericResponse<TechnicianDashboardDto> { Status = true, Data = data };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new GenericResponse<TechnicianDashboardDto> { Status= false, Error=ex.Message }; 
            }
        }

        [HttpGet("GetTechnicianWorkStats")]
        public async Task<OutputResponse> GetTechnicianStats()
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if(userId== null)
                {
                    return new OutputResponse { Status = false, Error = MessageHelper.Get("User", "UserId") };
                }
                else
                {
                    var data = await technicianService.GetTechnicianStats(userId);
                    return new OutputResponse { Status = true,Data=data};
                }
                    

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Data = ex.Message };
            }
        }


        [HttpGet("GetMonthlyUpdates")]
        public async Task<OutputResponse> GetMonthlyCompletedTasks()
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (userId == null)
                {
                    return new OutputResponse { Status = false, Error = MessageHelper.Get("User", "UserId") };
                }
                var data = await technicianService.GetMonthlyCompletedTasks(userId);
                return new OutputResponse { Status = true, Data = data };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [HttpGet("GetRevenue")]
        public async Task<OutputResponse> GetRevenue()
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (userId == null)
                {
                    return new OutputResponse { Status = false, Error = MessageHelper.Get("User", "UserId") };
                }
                var data = await technicianService.GetTechnicianRevenue(userId);
                return new OutputResponse { Status = true, Data = data };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false,Error=ex.Message };
            }
        }



        [HttpGet("GetEfficinecy")]
        public async Task<OutputResponse> GetTechnicianEfficiency()
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (userId == null)
                {
                    return new OutputResponse { Status = false, Error = MessageHelper.Get("User", "UserId") };
                }
                var data = await technicianService.GetTechnicianEfficiency(userId);
                return new OutputResponse { Status = true, Data = data };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }
    }
}
