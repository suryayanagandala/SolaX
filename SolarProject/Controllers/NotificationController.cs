using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.DTO_S;
using DbAccessaLayer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client.Extensibility;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SolarProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService notificationService;
        public NotificationController(IUnitOfWork unitOfWork)
        {
            notificationService=new NotificationService(unitOfWork);
        }

        [HttpGet("GetNotifications/{userId}")]
        public async Task<OutputResponse> GetNotifications([FromRoute] string userId)
        {
            try
            {
                var notifications=await notificationService.GetNotifiations(userId);
                return new OutputResponse { Status=true,Data=notifications };
            }
            catch (Exception ex)
            {
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [HttpGet("CountOfUnreadNotifications/{userId}")]
        public async Task<OutputResponse> GetUnreadMessages([FromRoute] string userId)
        {
            try
            {
                var count=await notificationService.CountUnreadNotifications(userId);
                return new OutputResponse {Status=true, Data=count };
            }
            catch(Exception ex)
            {
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }


        [HttpPatch("MarkAsRead/{notificationId}")]
        public async Task<OutputResponse> MarkasRead([FromRoute]int notificationId)
        {
            try
            {
                await notificationService.MarAsRead(notificationId);
                return new OutputResponse {Status=true,Data=MessageHelper.Get("Notifications", "MarkAsRead") };
            }
            catch(Exception ex)

            {
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [HttpPatch("MarkAllRead/{userId}")]

        public async Task<OutputResponse> MarkAllAsRead([FromRoute] string userId)
        {
            try
            {
                await notificationService.MarkAllAsRead(userId);
                return new OutputResponse { Status = true, Data = MessageHelper.Get("Notifications", "MarkAllRead") };
            }
            catch (Exception ex)
            {
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

       
        
    }
}
