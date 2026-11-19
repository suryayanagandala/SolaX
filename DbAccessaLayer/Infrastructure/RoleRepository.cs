using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using CoreModels.Entities;
using DbAccessaLayer.Helpers;
using iText.Signatures.Validation.Lotl;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DbAccessaLayer.Infrastructure
{
    public class RoleRepository : IRoleRepository
    {
        public ApplicationDbContext _db;
        public RoleRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<object>> GetAllPermissionBasedOnRole(string userId)
        {
            try
            {
                var userExists=await _db.Users.Include(x=>x.Role).Where(x=>x.UserId == userId).FirstOrDefaultAsync();
                string roleName = "";
                if (userExists==null)
                {
                    throw new NotFoundException(MessageHelper.Get("User", "UserId"));
                }

                if (userExists.RoleId == 2)
                {
                    var technicianRole=await _db.Technicians.Where(x=>x.UserId==userId).FirstOrDefaultAsync();

                    if (technicianRole == null)
                        throw new NotFoundException(MessageHelper.Get("Technician", "Invalid"));

                    roleName = technicianRole.Specialization
                        ?? throw new Exception("Invalid role");
                }
                else
                {
                    roleName = userExists.Role.RoleName;
                }

                return await (from r in _db.Set<Roles>() join a in _db.Set<AccessPermissions>() on r.RoleId equals a.RoleId
                              where r.RoleId==userExists.RoleId && a.RoleName==roleName && r.IsActive &&a.IsActive
                              select (object) new
                              {
                                  RoleId=r.RoleId, 
                                  RoleName=roleName,
                                  Icon=a.Icon,
                                  AccessName=a.AccessName,
                                  AccessId=a.AccessId,
                                  Url=a.Url,
                              }).ToListAsync();

             
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Roles>> GetAllRoles()
        {
            try
            {
                var data = await _db.Roles.ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<User> GetAllUsers()
        {
            try
            {
                var users = _db.Users.Where(x => x.IsActive).Select(x => new User
                {
                    UserId = x.UserId,
                }).ToList();
                return users;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<User> GetUsersFromUser(string role)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(role))
                    throw new ArgumentException("Role name is required");
                var user = (from r in _db.Set<Roles>()
                            join u in _db.Set<Users>() on r.RoleId equals u.RoleId
                            where r.RoleName == role && r.IsActive && u.IsActive && u.IsActive
                            select new User
                            {
                                UserId = u.UserId,
                                RoleId = r.RoleId,
                                FullName = u.FullName,
                                EmailId = u.EmailId,
                            }).ToList();
                if (!user.Any())
                    throw new Exception("No users found for this role");

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
