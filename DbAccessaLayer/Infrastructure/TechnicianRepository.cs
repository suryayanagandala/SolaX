using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using DbAccessaLayer.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DbAccessaLayer.Infrastructure
{
    public class TechnicianRepository : ITechnicianRepository
    {
        private readonly ApplicationDbContext _db;

        private readonly IEmailService _emailService;
        public TechnicianRepository(ApplicationDbContext db,IConfiguration configuration, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }



        //email set up

        //private async Task SendUpateForAssignTech(string email, string name)
        //{
        //    var mailRequest = new MailRequest
        //    {
        //        Email = email,
        //        Subject = "Appointed to Technician",
        //        EmailBody = $@"<div style='width:100%;padding:20px;'>
        //        <h2>Hi,{name}</h2>
        //        <p>Hearty welcome to SolarX!.Thank you for join us we hope you bring a lot of contribution</p> <br/>
        //        <b>SolarX Team</b>"



        //    };
        //    await _emailService.SendEmail(mailRequest);
        //}

        public async Task<string> GenerateUserId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string id;
            bool exists;
            do
            {
                id = new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());

                exists = await UserIdExists(id);

            }
            while (exists);

            return id;
        }

        private async Task<bool> UserIdExists(string id)
        {
            return await _db.Users.AnyAsync(x => x.UserId == id);
        }

        private async Task<bool> UserNameExists(string name)
        {
            return await _db.Users.AnyAsync(x=>x.FullName== name);
        }

        private async Task<bool>EmailIdExists(string email)
        {
            return await _db.Users.AnyAsync(x => x.EmailId == email);
        }
        public async Task AssignedTechnician(TechnicianCreateDto dto)
        {
            using var transactions = await _db.Database.BeginTransactionAsync();
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }
                string passsword = "Tech@2003";

                bool name = await UserNameExists(dto.FullName);
                if (name)
                {
                    throw new DataExistsException(MessageHelper.Get("User","Name"));
                }

                bool email = await EmailIdExists(dto.EmailId);
                if (email)
                {
                    throw new DataExistsException(MessageHelper.Get("User", "EmailId"));
                }

                var userData = new Users
                {
                    UserId = await GenerateUserId(),
                    EmailId = dto.EmailId,
                    PhoneNumber = dto.PhoneNumber,
                    RoleId = 2,
                    Password = BCrypt.Net.BCrypt.HashPassword(passsword),
                    FullName = dto.FullName,
                    Address = dto.Address,
                    CreatedBy = "Admin"
                };
                _db.Users.Add(userData);

                await _db.SaveChangesAsync();

                var technicans = new Technicians
                {
                    UserId = userData.UserId,
                    Specialization = dto.Specialization.ToString(),
                    Descipline = dto.Discipline.ToString(),
                    ExperienceYears = dto.ExperienceYears,
                    Qualification = dto.Qualification.ToString(),
                    Salary = dto.Salary,
                    Service_Price = dto.ServicePrice,
                    IsActive = true,
                    IsActiveDeliveries = 0
                };

               

                await _db.Technicians.AddAsync(technicans);
                await _db.SaveChangesAsync();
                await transactions.CommitAsync();
                //await SendUpateForAssignTech(user.EmailId, user.FullName);
            }
            catch (Exception)
            {
                await transactions.RollbackAsync();
                throw;
            }
        }

        public async Task<TechnicianResponse> GetTechnician(string userId)
        {
            try
            {
                var technician=await _db.Technicians.Where(x=>x.UserId == userId &&x.IsActive).Include(x=>x.User).FirstOrDefaultAsync();

                if (technician == null)
                {
                    throw new NotFoundException(MessageHelper.Get("User","UserId"));
                }
                return new TechnicianResponse
                {
                    TechnicianId = technician.TechnicianId,
                    UserId = userId,
                    TechnicianName = technician.User.FullName,
                    Specifications = technician.Specialization,
                    ActiveDeliveries = Convert.ToInt32(technician.IsActiveDeliveries),
                    Salary = technician.Salary,
                    ServicePrice = technician.Service_Price,
                    Specialization = technician.Specialization,
                    Qualifications=technician.Qualification,
                    ExperienceYears=technician.ExperienceYears,


                };

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TechnicianDashboardDto> GetDashboard(string technicianId)
        {
            try
            {
                var data = await GetTechnician(technicianId);
                if (data == null)
                    throw new Exception("Technician not found");


                var start = new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero);
                var end = start.AddDays(1);

                if (data.Specifications.Equals(TechnicianRoles.Installer.ToString(),StringComparison.OrdinalIgnoreCase))
                {
                    var technician = await _db.Technicians.FirstOrDefaultAsync(x => x.UserId == technicianId);

                    if (technician == null)
                        throw new Exception("Invalid technician");

                    var deliveryQuery = _db.Deliveries.Include(x => x.Order).Where(x =>x.AssignedToTechId == technician.TechnicianId &&x.IsActive);

                    return new TechnicianDashboardDto
                    {
                        PendingCount = await deliveryQuery.CountAsync(x =>x.Status == DeliveryStatus.Pending.ToString()),

                        OutForDeliveryCount = await deliveryQuery.CountAsync(x => x.Status == DeliveryStatus.OutForDelivery.ToString()),

                        DeliveredTodayCount = await deliveryQuery.Where(x => x.Status == DeliveryStatus.Delivered.ToString())
                          .Where(x => x.DeliveryAt != null).Where(x => x.DeliveryAt >= start && x.DeliveryAt < end).CountAsync(),

                        TotalActiveDeliveries = await deliveryQuery.CountAsync(x =>x.Status != DeliveryStatus.Delivered.ToString() &&x.Order.IsActive),

                        Specilization = data.Specifications
                    };
                }

               
                var maintenanceQuery = _db.MaintenanceTechnicians.Include(x => x.GetMaintenance).ThenInclude(x => x.MaintenanceReports).Where(x =>x.TechnicianId == data.TechnicianId &&x.IsActive);

                return new TechnicianDashboardDto
                {
                    PendingCount = await maintenanceQuery.CountAsync(j =>j.GetMaintenance.Status == MaintenanceStatus.Pending.ToString()),

                    AssignedCount = await maintenanceQuery.CountAsync(j =>j.GetMaintenance.Status == MaintenanceStatus.Assigned.ToString()),

                    InProgressCount = await maintenanceQuery.CountAsync(j =>j.GetMaintenance.Status == MaintenanceStatus.Inprogress.ToString()),

                    CompletedCount = await maintenanceQuery.CountAsync(j =>j.GetMaintenance.Status == MaintenanceStatus.Completed.ToString()),

                    DeliveredTodayCount = await maintenanceQuery.Where(j => j.GetMaintenance.Status == MaintenanceStatus.Completed.ToString())
                                         .Where(j => j.GetMaintenance.MaintenanceReports.Any(r => r.CreatedAt >= start && r.CreatedAt < end)).CountAsync(),

                    Specilization = data.Specifications
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateTechnicianProfile(UpdateTechnicianDto dto, string userId)
        {
            var details = await _db.Technicians.FirstOrDefaultAsync(x => x.UserId == userId);

            if (details == null)
                throw new NotFoundException("No technician found");

            details.ExperienceYears = dto.ExperienceYears;
            details.Service_Price = dto.ServicePrice;
            details.Salary = dto.Salary;

            await _db.SaveChangesAsync();
        }

        public async  Task<TechnicianWorkStatusDto> GetTechnicianWorkStatus(string userId)
        {
            try
            {
                var technicianDetails=await GetTechnician(userId);


                if (technicianDetails.Specialization == Specialization.Installer.ToString())
                {
                    var data = await _db.Deliveries.Include(x => x.AssignedToTech).Where(x => x.AssignedToTech.UserId == userId).ToListAsync();

                    return new TechnicianWorkStatusDto
                    {
                        Assigned = data.Count(x => x.Status == "Assigned"),
                        Pending = data.Count(x => x.Status == "Pending"),
                        Completed = data.Count(x => x.Status == "Delivered")
                    };
                }

                else
                {
                    var data = await _db.MaintenanceRequests.Include(r => r.Maintenances).ThenInclude(m => m.Technician)
                                  .Where(r => r.Maintenances.Any(m => m.Technician.UserId == userId)).ToListAsync();
                    return new TechnicianWorkStatusDto
                    {
                        Assigned = data.Count(x => x.Status == "Assigned"),
                        Pending = data.Count(x => x.Status == "Pending"),
                        Completed = data.Count(x => x.Status == "Completed")
                    };
                }
               
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<MonthlyCompletedJobsDto>> GetMonthlyCompletedJobs(string userId)
        {
            try
            {
                var technician = await GetTechnician(userId);

                var startOfYear = new DateTime(TimeUtility.CurrentYear(), 1, 1);

                if (technician.Specialization == Specialization.Installer.ToString())
                {
                    var jobs = await _db.Installations
                        .Where(x => x.delivery.AssignedToTech.UserId == userId
                                 && x.Status == "Completed"
                                 && x.CompletedDate >= startOfYear)
                        .ToListAsync();

                    return jobs
                        .GroupBy(x => x.CompletedDate!.Value.Month)
                        .Select(g => new MonthlyCompletedJobsDto
                        {
                            Month = new DateTime(1, g.Key, 1).ToString("MMM"),
                            CompletedJobs = g.Count()
                        })
                        .OrderBy(x => DateTime.ParseExact(x.Month, "MMM", null).Month)
                        .ToList();
                }
                else
                {
                    var jobs = await _db.MaintenanceRequests
                        .Where(r => r.Maintenances.Any(m => m.Technician.UserId == userId)
                                 && r.Status == "Completed"
                                 && r.CreatedAt >= startOfYear)
                        .ToListAsync();

                    return jobs
                        .GroupBy(x => x.CreatedAt.Month)
                        .Select(g => new MonthlyCompletedJobsDto
                        {
                            Month = new DateTime(1, g.Key, 1).ToString("MMM"),
                            CompletedJobs = g.Count()
                        })
                        .OrderBy(x => DateTime.ParseExact(x.Month, "MMM", null).Month)
                        .ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<TechnicianRevenueDto>> GetTechnicianRevenue(string userId)
        {
            try
            {
                var technician = await GetTechnician(userId);

                if (technician == null)
                    throw new NotFoundException("User not found");

                var startOfYear = new DateTime(TimeUtility.CurrentYear(), 1, 1);

                if (technician.Specialization == Specialization.Installer.ToString())
                {
                    var jobs = await _db.InstallationTechnicians
                        .Where(x => x.Technician.UserId == userId
                            && x.Installation.Status == "Completed"
                            && x.Installation.CompletedDate >= startOfYear)
                        .Select(x => new
                        {
                            CompletedDate = x.Installation.CompletedDate,
                            Price = x.ServicePrice
                        })
                        .ToListAsync();

                    var result = jobs
                        .Where(x => x.CompletedDate != null)
                        .GroupBy(x => x.CompletedDate.Value.Month)
                        .Select(g => new TechnicianRevenueDto
                        {
                            Month = new DateTime(1, g.Key, 1).ToString("MMM"),
                            Revenue = g.Sum(x => x.Price)
                        })
                        .OrderBy(x => DateTime.ParseExact(x.Month, "MMM", null).Month)
                        .ToList();

                    return result;
                }
                else
                {
                    var jobs = await _db.MaintenanceTechnicians
                        .Where(x => x.Technician.UserId == userId
                            && x.GetMaintenance.Status == "Completed"
                            && x.GetMaintenance.MaintenanceReports
                                .Any(r => r.CreatedAt >= startOfYear))
                        .Select(x => new
                        {
                            CompletedDate = x.GetMaintenance.MaintenanceReports
                                .OrderBy(r => r.CreatedAt)
                                .Select(r => r.CreatedAt)
                                .FirstOrDefault(),
                            Price = x.ServicePrice
                        })
                        .ToListAsync();

                    var result = jobs
                        .Where(x => x.CompletedDate != default)
                        .GroupBy(x => x.CompletedDate.Month)
                        .Select(g => new TechnicianRevenueDto
                        {
                            Month = new DateTime(1, g.Key, 1).ToString("MMM"),
                            Revenue = g.Sum(x => x.Price)
                        })
                        .OrderBy(x => DateTime.ParseExact(x.Month, "MMM", null).Month)
                        .ToList();

                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<TechnicianEfficiencyDto> GetTechnicianEfficiency(string userId)
        {
            try
            {
                var technician = await GetTechnician(userId);

                if (technician.Specialization == Specialization.Installer.ToString())
                {
                    var jobs = await _db.InstallationTechnicians
                        .Where(x => x.Technician.UserId == userId)
                        .Select(x => x.Installation.Status)
                        .ToListAsync();

                    var total = jobs.Count;
                    var completed = jobs.Count(x => x == "Completed");

                    return new TechnicianEfficiencyDto
                    {
                        TotalJobs = total,
                        CompletedJobs = completed,
                        Efficiency = total == 0 ? 0 : (completed * 100.0) / total
                    };
                }
                else
                {
                    var jobs = await _db.MaintenanceTechnicians
                        .Where(x => x.Technician.UserId == userId)
                        .Select(x => x.GetMaintenance.Status)
                        .ToListAsync();

                    var total = jobs.Count;
                    var completed = jobs.Count(x => x == "Completed");

                    return new TechnicianEfficiencyDto
                    {
                        TotalJobs = total,
                        CompletedJobs = completed,
                        Efficiency = total == 0 ? 0 : (completed * 100.0) / total
                    };
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
