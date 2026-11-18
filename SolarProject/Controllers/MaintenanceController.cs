using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using DbAccessaLayer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SolarProject.Hubs;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SolarProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MaintenanceController : ControllerBase
    {
        private readonly MaintenanceService maintenanceService;
        private ILogger<MaintenanceController> logger;
        
        public MaintenanceController(IUnitOfWork unitOfWork, ILogger<MaintenanceController> logger, ISignalRRepository signalRRepository)
        {
            maintenanceService = new MaintenanceService(unitOfWork,signalRRepository);
            this.logger = logger;
          
        }

        [HttpGet("GetMaintenaceView/{userId}")]
        public OutputResponse GetMaintenance([FromRoute] string userId)
        {
            try
            {
                var data=maintenanceService.GetMaintenanceForUser(userId);
                logger.LogInformation($"Fetching maintenances by {userId} at {TimeUtility.NowIST()}");
                return new OutputResponse { Status=true, Data = data };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status=false,Error=ex.Message};
            }
        }

        [HttpPost("RaiseRequest")]
        public async Task<OutputResponse> RaiseRequest([FromBody] MaintenanceRequestCreateDto dto)
        {
            try
            {
                await maintenanceService.RaiseMaintenanaceRequest(dto);
                logger.LogInformation($"User has sent maintennace request of{dto.UserId} and {dto.InstallationId}");
                return new OutputResponse { Status = true, Data = MessageHelper.Get("Request", "Success") };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [Authorize(Roles="Technician")]
        [HttpGet("GetMaintenances/{userId}")]
        public  async Task<OutputResponse> GetMaintennaces([FromRoute] string userId)
        {
            try
            {
                var result=await maintenanceService.GetMaintenanceJods(userId);
                logger.LogInformation($"technician is fetching the data of {userId} at {DateTime.Now}");
                return new OutputResponse { Status = true, Data = result };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [Authorize(Roles="Technician")]
        [HttpPost("MaintenanceReport")]
        public async Task<OutputResponse> UploadMaintenenceReport([FromForm] MaintenanceReportDto maintenanceReport)
        {
            try
            {
                await maintenanceService.UploadMaintennaceStatus(maintenanceReport);
                logger.LogInformation($"Technician is completed mainatinnece of{maintenanceReport.RequestId}");
                return new OutputResponse { Status = true, Data = MessageHelper.Get("Maintenance", "Success") };
            }
            catch(Exception ex)
            {
                var errormessages = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                logger.LogError(errormessages);
                return new OutputResponse { Status = false, Error = errormessages };
            }
        }

        [HttpGet("GetCompletedMaintenanaces")]
        public async Task<OutputResponse> GetMaintenances()
        
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (userId != null)
                {
                    var data = await maintenanceService.GetCompletedMaintenanaces(userId);
                    logger.LogInformation($"Get Completed Maintenanaces id {userId}");
                    return new OutputResponse { Status = true, Data = data };
                }
                return new OutputResponse { Status = false, Error = MessageHelper.Get("User", "UserId") };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }
    }
}
