using CoreModels.DTO_S;
using CoreModels.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Roles>> GetAllRoles();

        List<User> GetUsersFromUser(string role);

        List<User> GetAllUsers();

        Task<List<object>> GetAllPermissionBasedOnRole(string userId);
    }
}
