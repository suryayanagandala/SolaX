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
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SolarProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private ILogger<PaymentController> _logger;
        private readonly IHubContext<NotificationHub> hubContext;
        private CartRepositoryService cartService;
        public PaymentController(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hub, ILogger<PaymentController> logger)
        {
            _paymentService = new PaymentService(unitOfWork);
            _logger = logger;
            hubContext = hub;
            cartService = new CartRepositoryService(unitOfWork);
        }

        [HttpPost("PaymentGateway")]
        public async Task<OutputResponse> PaymentGateway([FromBody]PaymentCreateDto dto)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if(userId == null)
                {
                    return new OutputResponse { Status = false, Error = MessageHelper.Get("User", "UserNotFound") };
                }
               var payment= await _paymentService.PaymentGateway(dto);

                var count = await cartService.GetCountOfCartItems(userId);

                await hubContext.Clients.User(userId)
                    .SendAsync("CarItemsCount", new
                    {
                        cartCount = count
                    });
                _logger.LogInformation("Payment is successfully created by" +dto.UserId);
                return new OutputResponse { Status = true, Data=MessageHelper.Get("Payment", "PaymentSuccess") +dto.PaymentMethod };
            }
            catch (Exception ex)
            {
                var errorMessages = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _logger.LogError(errorMessages);
                return new OutputResponse  { Status = false, Error = errorMessages };
            }
        }

        [HttpGet("GetQrCode/{orderId}")]
        public async Task<OutputResponse> GenerateQrResponse([FromRoute] int orderId)
        {
            try
            {
                var data=await _paymentService.GenerateQr(orderId);
                _logger.LogInformation($"{orderId} {data} generated at {TimeUtility.NowIST()}");
                return new OutputResponse { Status = true, Data=data };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new OutputResponse { Status=false, Error=ex.Message };
            }
        }
    }
}
