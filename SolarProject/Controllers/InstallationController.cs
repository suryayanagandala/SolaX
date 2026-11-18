using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using DbAccessaLayer.Helpers;
using iText.Kernel.Pdf.Tagutils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SolarProject.Hubs;
using System.Runtime.InteropServices;

namespace SolarProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles="Technician")]
    public class InstallationController : ControllerBase
    {
        private readonly InstallationService installationService;
        private readonly RoleService roleService;
        private ILogger<InstallationController> logger;

        public InstallationController(IUnitOfWork unitOfWork,ISignalRRepository signalRRepository, ILogger<InstallationController> logger)
        {
            installationService = new InstallationService(unitOfWork, signalRRepository);
            roleService = new RoleService(unitOfWork);
            this.logger = logger;
        }

        [HttpGet("GetInstallationStatus")]
        public async Task<OutputResponse> GetInstallationStatus()
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (userId == null)
                {
                    return new OutputResponse { Status = false, Error = MessageHelper.Get("User", "UserId") };
                }

                var data=await installationService.GetInsttalationStatus(userId);

                logger.LogInformation($"Technician getting insttalation status of{userId} at {DateTime.Now}");

                return new OutputResponse { Status = true, Data = data };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse  { Status = false, Error = ex.Message};
            }
        }

        [HttpPatch("InstallationStatus/{installationId}")]
        public async Task<OutputResponse> MarkedAsStatus([FromRoute] string installationId)
        {
            try
            {
                await installationService.MarkasProgress(installationId);

                logger.LogInformation($"{installationId} has been marked as progress at {DateTime.Now}");
                return new OutputResponse { Status= true,Data=MessageHelper.Get("Installation","Updated") };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [HttpPost("InstallationReport")]
        public async Task<OutputResponse> CompleteInstallation([FromForm] InstallationReportDto dto)
        {
            try
            {
              
                var techinician= await installationService.CompletedInstallation(dto);

                logger.LogInformation($"Instllation Id {dto.InstallationId} has successfully completed by {techinician}");
                return new OutputResponse { Status = true, Data = MessageHelper.Get("Installation", "Completed") };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                var errorMessages = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new OutputResponse { Status=false,Error = errorMessages };
            }
        }
    }
}
