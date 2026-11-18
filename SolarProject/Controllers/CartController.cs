using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.DTO_S;
using DbAccessaLayer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SolarProject.Hubs;
using System;

namespace SolarProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly CartRepositoryService cartRepositoryService;

        private readonly IHubContext<NotificationHub> hubContext;

        private ILogger<CartController> logger; 
        public CartController(IUnitOfWork unitOfWork,IHubContext<NotificationHub> hubContext,ILogger<CartController> logger)
        {
            cartRepositoryService = new CartRepositoryService(unitOfWork);
            this.hubContext = hubContext;
            this.logger = logger;
        }

        [HttpPost("AddToCart")]
        public async Task<OutputResponse>AddToCart(AddToCartDto dto)
        {
            try
            {
                await cartRepositoryService.AddToCartAsync(dto);
                var count = await cartRepositoryService.GetCountOfCartItems(dto.UserId);

                await hubContext.Clients.User(dto.UserId)
                    .SendAsync("CarItemsCount", new
                    {
                        cartCount = count
                    });
                logger.LogInformation("sucessfully added into cart" + dto.UserId);
                return new OutputResponse { Status = true, Data = MessageHelper.Get("Cart", "ItemAdded") };
            }
            catch(DbUpdateException ex)
            {
                var errormEssages = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                logger.LogError(errormEssages);
                return new OutputResponse    { Status=false,Error= errormEssages };
            }
        }

        [HttpPatch("UpdateCart")]
        public async Task<OutputResponse> UpdateToCart(UpdateCartItemDto dto)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (userId == null)
                {
                    return new OutputResponse { Status = false, Error = MessageHelper.Get("User", "UserId") };
                }
                await cartRepositoryService.UpdateCartAsync(dto);
                logger.LogInformation("User update cart at" + dto.Quantity);
                var count = await cartRepositoryService.GetCountOfCartItems(userId);
                await hubContext.Clients.User(userId)
                   .SendAsync("CarItemsCount", new
                   {
                       cartCount = count
                   });
                logger.LogInformation("sucessfully added into cart" + userId);
                return new OutputResponse { Status = true, Data = MessageHelper.Get("Cart", "ItemUpdated") };
            }
            catch (DbUpdateException ex)
            {
                var errormEssages = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                logger.LogError(errormEssages);
                return new OutputResponse { Status = false, Error = errormEssages };
            }
        }

        [HttpDelete("RemoveCart/{cartItemId}")]
        public async Task<OutputResponse> RemoveCart(int cartItemId)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (userId == null)
                {
                    return new OutputResponse { Status = false, Error = MessageHelper.Get("User", "UserId") };
                }
                await cartRepositoryService.RemoveCartItem(cartItemId);

                var count = await cartRepositoryService.GetCountOfCartItems(userId);

                await hubContext.Clients.User(userId)
                    .SendAsync("CarItemsCount", new
                    {
                        cartCount = count
                    });

                logger.LogInformation("Successfully remoce cartItem containing of "+cartItemId);
                return new OutputResponse { Status = true, Data = MessageHelper.Get("Cart", "ItemRemoved") };
            }
            catch (Exception ex)
            {
                logger.LogError($"{ex.Message}", ex);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }


        [HttpGet("UserCart")]
        public async Task<OutputResponse> GetUserCart(string userId)
        {
            try
            {
                var data=await cartRepositoryService.GetUserCart(userId);
                logger.LogInformation("Get user cart of "+userId,"at "+DateTime.Now);
                return new OutputResponse {Status = true, Data = data};   
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return new OutputResponse { Status=false, Error = ex.Message };
            }
        }

        [HttpGet("CountOfCartItems/{userId}")]
        public async Task<GenericResponse<int>> CountOfCartItems([FromRoute] string userId)
        {
            try
            {
                int count=await cartRepositoryService.GetCountOfCartItems(userId);

               

                return new GenericResponse<int> {Status = true, Data = count };
                
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return new GenericResponse<int> { Status= false, Error = ex.Message };
            }
        }
    }
}
