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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using SolarProject.Hubs;
using System.Runtime.InteropServices;

namespace SolarProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly OrderService orderService;
        private ILogger<OrderController> logger;

        public OrderController(IUnitOfWork unitOfWork, ILogger<OrderController> logger)
        {
            orderService = new OrderService(unitOfWork);
            this.logger = logger;
        }

        [HttpGet("OrderHistory")]
        public async Task<OutputResponse> OrderHistory()
        {
            try
            {
                var userId=User.FindFirst("UserId")?.Value;

                if(userId == null)
                {
                    return new OutputResponse { Status = false, Data = MessageHelper.Get("Order", "NotFound") };
                }

                var data=await orderService.OrderHistory(userId);
                logger.LogInformation("user is getting order hsitory of" + userId);

                return new OutputResponse { Status=true, Data = data };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [HttpGet("OrderDetails/{orderId}")]
        public async Task<OutputResponse> OrderDetails(int orderId)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;

                if (userId == null)
                {
                    return new OutputResponse { Status = false, Data = MessageHelper.Get("Order", "NotFound") };
                }

                var data= await orderService.GetOrderDetails(userId, orderId);
                return new OutputResponse { Status=true,Data = data };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status= false, Error = ex.Message };
            }
        }

        [HttpPost("PlaceOrder")]
        public async Task<OutputResponse>PlaceOrder(OrderCreateDto dto)
        {
            try
            {
                var data = await orderService.PlaceOrder(dto);

                logger.LogInformation("User place order " + dto.UserId);
                return new OutputResponse {Status=true,Data=data };
            }
            catch(DbUpdateException ex)
            {
                logger.LogError(ex.Message);
                var errorMessages=ex.InnerException!=null?ex.InnerException.Message:ex.Message;
                return new OutputResponse { Status=false,Error = errorMessages };
            }
        }

        [HttpPatch("CancelOrder/{orderId}")]
        public async Task<OutputResponse> CancelOrder([FromRoute]int orderId)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;

                if (userId == null)
                {
                    return new OutputResponse { Status = false, Data = MessageHelper.Get("Order", "NotFound") };
                }

                else
                {
                    await orderService.CancelOrder(orderId, userId);

                    logger.LogInformation("User can celled order of " + orderId);
                    return new OutputResponse { Status = true, Data = MessageHelper.Get("Order", "OrderCancelled") };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status=false,Error = ex.Message };
            }
        }


        [HttpGet("UserDetails/{userId}")]
        public async Task<OutputResponse>GetUserDetails([FromRoute]string userId)
        {
            try
            {
                var userData = await orderService.GetUserDetails(userId);

                return new OutputResponse { Status = true, Data = userData };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        

        [HttpGet("TrackDelivery/{orderId}")]
        public async Task<OutputResponse> TrackOrder([FromRoute] int orderId)
        {
            try
            {
                var data=await orderService.TrackDelivery(orderId);
                logger.LogInformation($"User track delivery at {DateTime.UtcNow}");
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
