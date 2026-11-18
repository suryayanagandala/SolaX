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

namespace SolarProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Technician")]
    public class DeliveryController : ControllerBase
    {
        private readonly DeliveryService deliveryService;
        private readonly ILogger<DeliveryController> logger;
        public DeliveryController(IUnitOfWork unitOfWork,ILogger<DeliveryController> logger)
        {
           deliveryService= new DeliveryService(unitOfWork);
            this.logger = logger;
        }

        [HttpPatch("MarkAsCompletedDelivery/{deliveryId}")]
        public async Task<OutputResponse> MarkAsCompletedDelivery(int deliveryId)
        {
            try
            {
                var userId= await deliveryService.MarkAsCompletedDelivery(deliveryId);
                logger.LogInformation($"{deliveryId} has successfully delivered at {DateTime.Now}");
                return new OutputResponse { Status = true, Data = MessageHelper.Get("Delivery", "Delivered") };
            }
            catch (Exception ex)
            {
                var errorMessages = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                logger.LogError(errorMessages);
                return new OutputResponse { Status = false, Error = errorMessages };
            }
        }


        [HttpPatch("MarkAsOutOfDelivery/{deliveryId}")]
        public async Task<OutputResponse> MarkAsOutOfDelivery(int deliveryId)
        {
            try
            {
                await deliveryService.MarkAsOutOfDelivery(deliveryId);
                logger.LogInformation($"{deliveryId} has successfully outFordelivered at {DateTime.Now}");
                return new OutputResponse { Status = true, Data = MessageHelper.Get("Delivery", "OutOfDelivery") };
            }
            catch (Exception ex)
            {
                var errorMessages = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                logger.LogError(errorMessages);
                return new OutputResponse  { Status = false, Error = errorMessages };
            }
        }

        [HttpGet("GetOutOfStock")]
        public async Task<OutputResponse> GetOutOfStock()
            {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (userId == null)
                    return new OutputResponse { Status = false, Error = MessageHelper.Get("User", "UserId") };

                var data=await deliveryService.GetOutForDeliveryStock(userId);
                return new OutputResponse { Status=true,Data = data };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [HttpGet("DeliveredOrders")]
        public async Task<OutputResponse> GetDelivered()
        {
            try
            {
                var userId=User.FindFirst("UserId")?.Value;
                if (userId == null)
                    return new OutputResponse { Status = false, Error = MessageHelper.Get("User", "UserId") };
                var data=await deliveryService.GetDeliveredOrders(userId);
                return new OutputResponse { Status=true, Data = data };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message};
            }
        }
    }
}
