using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using DbAccessaLayer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace SolarProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AuthController : ControllerBase
    {
        private readonly UserService userService;

        private readonly TechnicianService technicianService;

        private readonly AdminService adminService;

        private readonly ProductService productService;

        private ILogger<AuthController> logger;
        public AuthController(IUnitOfWork unitOfWork, ILogger<AuthController> logger)
        {
            userService = new UserService(unitOfWork);

            technicianService = new TechnicianService(unitOfWork);

            adminService = new AdminService(unitOfWork);

            productService = new ProductService(unitOfWork);
            this.logger = logger;
        }
        [HttpPut("UpdateProfile")]
        public async Task<OutputResponse>UpdateProfile(UpdateProfileDTO dto)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return new OutputResponse { Status=false, Data = "Invalid userId" };

                await userService.UpdateProfile(userId, dto);
                return new OutputResponse { Status = true, Data = MessageHelper.Get("User", "Profile") };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Data = ex.Message };
            }
        }

        //[Authorize(Roles="Admin")]
        [HttpPost("AssignedTechnican")]
        public async Task<OutputResponse> AssignedTechnician([FromBody]TechnicianCreateDto dto)
        {
            try
            {
                await technicianService.CreateTechnician(dto);
                return new OutputResponse { Status = true, Data = MessageHelper.Get("Technician", "TechnicianAssigned") };
            }
            catch(DbUpdateException ex)
            {
                var errorMessage= ex.InnerException!=null ? ex.InnerException.Message : ex.Message;
                logger.LogError(errorMessage);
                return new OutputResponse { Status = false, Error = errorMessage };
            }
        }

        [Authorize(Roles="Admin")]
        [HttpGet("GetAllstats")]
        public async Task<OutputResponse> GetStats()
        {
            try
            {
                var data = await adminService.GetStatsData();
                return new OutputResponse { Status = true, Data=data };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [Authorize(Roles="Admin")]
        [HttpGet("GetAllCustomer")]
        public async Task<OutputResponse> GetAllCustomers()
        {
            try
            {
                var getCustomersData = await this.adminService.GetAllUsers();
                return new OutputResponse { Status = true, Data = getCustomersData };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [Authorize(Roles="Admin")]
        [HttpPost("AddProducts")]
        public async Task<OutputResponse> AddProduct(ProductResponse dto)
        {
            try
            {
                await productService.AddProducts(dto);
                return new OutputResponse { Status = true, Data = dto };
            }
            catch (DbUpdateException ex)
            {
                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                return new OutputResponse { Status = false, Error = errorMessage };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("updateProducts/{productId}")]
        public async Task<OutputResponse> UpadateProductDetails(int productId, [FromBody] ProductResponse dto)
        {
            try
            {
                await productService.UpdateProduct(productId, dto);
                return new OutputResponse { Status = true,Data=dto };
            }
            catch(DbUpdateException ex)
            {
                var erroMessages = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                logger.LogError(erroMessages);
                return new OutputResponse { Status = false, Error = erroMessages };
            }
            
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{productId}/images")]
        public async Task<OutputResponse> AddImagesForProducts(int productId, [FromForm] List<IFormFile> images)
        {
            try
            {  
                await productService.AddProductImages(productId, images);
                return new OutputResponse { Status =true,Data=MessageHelper.Get("ProductImages", "Success") };
            }

            catch(DbUpdateException ex)
            {
                var erroMessages = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new OutputResponse { Status = false, Error = erroMessages };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [Authorize(Roles="Admin")]
        [HttpPut("UpdateImages/{imageId}")]
        public async Task<OutputResponse> GetIamgesById(int imageId,IFormFile file)
        {
            try
            {
               await  productService.UpdatePrpductImage(imageId, file);
                return new OutputResponse { Status = true, Data =MessageHelper.Get("ProductImages", "Update") };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [Authorize(Roles="Admin")]
        [HttpGet("GetImagesByProductId/{productId}")]
        public async Task<OutputResponse> GetAllImagesByProductId(int productId)
        {
            try
            {
                var imagesData = await productService.GetIamgesById(productId);
                return new OutputResponse
                {
                    Status = true,
                    Data = imagesData
                };
            }
            catch (DbUpdateException ex)
            {
                var errorMessages=ex.InnerException!=null?ex.InnerException.Message:ex.Message;

                return new OutputResponse
                {
                    Status = false,
                    Error = errorMessages
                };
            }
        }


        [Authorize(Roles="Admin")]
        [HttpDelete("DeleteImage/{imageId}")]
        public async Task<OutputResponse> DeleteImage([FromRoute] int imageId)
        {
            try
            {
                await productService.DeleteImageById(imageId);
                return new OutputResponse    { Status = true, Data = MessageHelper.Get("ProductImages", "Deleted") };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [Authorize(Roles="Admin")]
        [HttpGet("GetAllProducts")]
        public async Task<OutputResponse> GetAllProducts()
        {
            try
            {
                var data=await productService.GetAllProducts();
                return new OutputResponse { Status = true, Data = data };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet("GetProductDetails/{productId}")]

        public async Task<OutputResponse>GetProductById(int productId)
        {
            try
            {
                var productData=await productService.GetProductById(productId);
                return new OutputResponse { Status = true, Data = productData };
            }
            catch (DbUpdateException ex)
            {
                var errorMessages = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new OutputResponse { Status = false, Error = errorMessages };
            }
        }

       

        [Authorize(Roles = "Admin")]
        [HttpPatch("soft-delete")]
        public async Task<OutputResponse>DeleteProduct(SoftDeleteProductDto dto)
        {
            try
            {
                await productService.SoftDeleteProduct(dto);
                return new OutputResponse { Status = true, Data = MessageHelper.Get("Products", "Deleted") };
            }
            catch(Exception ex)
            {
                 logger.LogError(ex.Message);
                return new OutputResponse { Status=false, Error = ex.Message };    
            }
        }

        [HttpPatch("restore/{productId}")]
        public async Task<OutputResponse> RestoreProduct(int productId)
        {
            try
            {
                await productService.RestoreProduct(productId);
                return new OutputResponse { Status = true, Data = MessageHelper.Get("Products", "Activated") };
            }
            catch (Exception ex)
            {
                 logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [HttpGet("GetProductionData")]
        public async Task<OutputResponse> GetProductionData()
        {
            try
            {
                var data=await adminService.GetProductionData();
                return new OutputResponse {Status=true, Data = data };
            }
            catch(Exception ex)
            {
                 logger.LogError(ex.Message);
                return new OutputResponse { Status = false,Error = ex.Message};
            }
        }

        [HttpGet("GetProductionTrend")]
        public async Task<OutputResponse> ProductionTrend()
        {
            try
            {
                var data=await adminService.GetProductionTrend();
                return new OutputResponse { Status=true, Data = data };
            }
            catch (Exception ex)
            {
                 logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [HttpGet("ProductionUpdate")]
        public async Task<OutputResponse> GetProductionUpdate()
        {
            try
            {
                var data = await adminService.GetProductionUpdate();
                return new OutputResponse { Status=true,Data = data };
            }
            catch (Exception ex)
            {
                 logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [HttpGet("SoldProducts")]
        public async Task<OutputResponse> SoldProductsData()
        {
            try
            {
                var data = await adminService.SoldProducts();
                return new OutputResponse
                {
                    Status = true,
                    Data = data
                };
            }
            catch(Exception ex)
            {
                return new OutputResponse { Status=false, Error = ex.Message};  
            }
        }


        [HttpPut("UpdateTechnicianProfile/{userId}")]
        public async Task<OutputResponse> UpdateTechnicianProfile([FromRoute] string userId, [FromBody] UpdateTechnicianDto dto)
        {
            try
            {
                await technicianService.UpdateTechnicianProfile(dto, userId);
                logger.LogInformation("succesfully upadte profile technician of" + userId);
                return new OutputResponse { Status = true };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }


        [HttpGet("TechnicianData/{userId}")]
        public async Task<OutputResponse> GetTechnicianData([FromRoute] string userId)
        {
            try
            {
                var data = await technicianService.GetTechncianResponse(userId);
                return new OutputResponse { Status = true, Data = data };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }


        [HttpGet("GetProductionUpdatesWithAi")]
        public async Task<OutputResponse> GetProductionAiUpdates()
        {
            try
            {
                var result=await adminService.GetProductionUpdates();
                logger.LogInformation($"{result}");
                return new OutputResponse { Status = true,Data=result };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Data = ex.Message };
            }
        }

    }
}
