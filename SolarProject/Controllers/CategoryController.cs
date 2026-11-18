using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.DTO_S;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SolarProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles="Admin")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService categoryService;
        private ILogger<CategoryController> logger;
        public CategoryController(IUnitOfWork unitOfWork, ILogger<CategoryController> logger)
        {
            categoryService = new CategoryService(unitOfWork);
            this.logger = logger;
        }

        [HttpGet("GetAllCategories")]
        public async Task<OutputResponse> GetAllCategories()
        {
            try
            {
                var data= await categoryService.GetAllCategories();
                logger.LogInformation("fetching the categories at" + DateTime.Now.ToString());
                return new OutputResponse { Status=true, Data = data };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [HttpGet("GetCategoryListById/{id}")]
        public async Task<OutputResponse> GetCategoryListById(int id)
        {
            try
            {
                var data=await categoryService.GetCategoryListById(id);
                logger.LogInformation($"Get Category list based on {id}");
                return new OutputResponse { Status=true, Data = data };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [HttpPost("UploadCategory")]
        public async  Task<OutputResponse> CreateCategory([FromForm] CategoryDto dto)
        {
            try
            {
                var data = await categoryService.CreateCategory(dto);
                logger.LogInformation("New category is added at"+DateTime.Now);
                return new OutputResponse {Status=true, Data = data };
            }
            catch(DbUpdateException ex)
            {
                var erroMessages=ex.InnerException!=null?ex.InnerException.Message:ex.Message;
                logger.LogError(erroMessages);
                return new OutputResponse {Status=false, Error=erroMessages};
            }
        }

        [HttpPatch("UpdateCategory/{id}")]
        public async Task<OutputResponse> UpdateCategory(int id, [FromForm] CreateUpdateCategoryDto dto)
        {
            try
            {
                var data= await categoryService.UpdateCategory(id,dto);
                logger.LogInformation($"Upload Category is updated at {DateTime.Now} of {id} with {dto.Name}");
                return new OutputResponse {Status =true, Data = data };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse    { Status=false, Error=ex.Message};
            }
        }
        


     


        //[HttpGet("GetChildrens")]
        //public async Task<OutputResponse> GetCategoryTree()
        //{
        //    try
        //    {
        //        var data=await categoryService.GetCategoryTree();
        //        return new OutputResponse { Status = true, Data = data };
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex.Message);
        //        return new OutputResponse { Status = false, Error = ex.Message };
        //    }
        //}


        [HttpPatch("DeleteCategory")]
        public async Task<OutputResponse> DeleteCategory(SoftDeleteCategoryDto dto)
        {
            try
            {
                 await categoryService.SoftDeleteCategory(dto);
                logger.LogInformation("Delete Category of"+dto.CategoryId);
                return new OutputResponse { Status = true, Data = $"successfully deactivated of{dto.CategoryId}" };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status=false, Error = ex.Message };
            }
        }

        [HttpPatch("RestoreCategory/{categoryId}")]
        public async Task<OutputResponse> RestoreCategory(int categoryId)
        {
            try
            {
                await categoryService.RestoreCategory(categoryId);
                logger.LogInformation($"Restore Category of {categoryId} at {DateTime.Now}");
                return new OutputResponse { Status = true, Data = $"Successfully activated of containing id as {categoryId}" };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [HttpPatch("ParentCategory/{categoryId}")]
        public async Task<OutputResponse> UpdateParentCategory(int categoryId, CreateUpdateCategoryDto dto)
        {
            try
            {
                await categoryService.UpdateParentCategory(categoryId, dto);
                logger.LogInformation("Parent Category is updated at" + categoryId);
                return new OutputResponse { Status = true, Data = $"Category updated as {dto.Name}" };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [HttpPatch("DeleteParentCategory")]
        public async Task<OutputResponse>DeletepParentCategory([FromBody]SoftDeleteCategoryDto dto)
        {
            try
            {
                await categoryService.SoftDeleteParentCategory(dto);
                logger.LogInformation("Delete ParentCategory of" + dto.CategoryId);
                return new OutputResponse { Status = true, Data = $"Successfully deactivated of category as {dto.CategoryId}" };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse {Status = false, Error = ex.Message};
            }
        }

        [HttpPatch("RestoreParentCategory/{categoryId}")]
        public async Task<OutputResponse> RestoreParentCategory(int categoryId)
        {
            try
            {
                await categoryService.RestoreParentCategory(categoryId);
                logger.LogInformation($"Restore ParentCategory of {categoryId} at {DateTime.Now}");
                return new OutputResponse { Status = true, Data = $"Successfully activated of containing id as {categoryId}" };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }
    }
}
