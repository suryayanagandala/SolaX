using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class RoleService 
    {
        private IUnitOfWork unitOfWork;
        public RoleService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Roles>> GetRolesAsync()=>await this.unitOfWork.RoleRepository.GetAllRoles();

        public List<User>GetUsersFromRole(string role)
        {
            return this.unitOfWork.RoleRepository.GetUsersFromUser(role);
        }

        public List<User>GetAllUsers()=>this.unitOfWork.RoleRepository.GetAllUsers();

        public async Task<List<object>> GetAccessPermissions(string userId) => await this.unitOfWork.RoleRepository.GetAllPermissionBasedOnRole(userId);
    }
}
