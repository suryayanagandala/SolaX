using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using DbAccessaLayer.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SolarProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private RoleService roleService;
        private readonly ILogger<RoleRepository> logger;
        public RolesController(IUnitOfWork unitOfWork,ILogger<RoleRepository> logger)
        {
            this.roleService = new RoleService(unitOfWork);
            this.logger= logger;
        }

        [HttpGet("GetAllRoles")]
        public async Task<OutputResponse> GetAllRoles()
        {
            try
            {
                var data = await this.roleService.GetRolesAsync();
                logger.LogInformation("getting all roles at"+DateTime.Now);
                return new OutputResponse{ Status = true, Data = data };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [HttpGet("GetUserFromRole/{role}")]
        public OutputResponse GetData([FromRoute] string role)
        {
            try
            {
                var data=roleService.GetUsersFromRole(role);
                logger.LogInformation("Grt informations of all users based on "+role);
                return new OutputResponse { Status = true, Data = data };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status=false, Error = ex.Message };
            }
        }

        [HttpGet("GetAllUsers")]
        public OutputResponse GetAllUsers()
        {
            try
            {
                var allUsers=roleService.GetAllUsers();
                return new OutputResponse { Status=true, Data = allUsers }; 
            }
            catch(Exception ex)
            {
                return new OutputResponse { Status=false,Error=ex.Message};
            }
        }

        [HttpGet("AccessPermissions/{userId}")]
        public async Task<OutPutResponse<object>> GetAccessPermisions([FromRoute] string userId)
        {
            try
            {
               var data=await roleService.GetAccessPermissions(userId);
                return new OutPutResponse<object> { Status=true, Data = data };

            }
            catch(Exception ex)
            {
                return new OutPutResponse<object> { Status=false,Error = ex.Message};
            }
        }
    }
}
