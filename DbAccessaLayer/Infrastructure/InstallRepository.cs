using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using DbAccessaLayer.Helpers;
//using iText.Kernel.Pdf.Tagutils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure
{
    public class InstallRepository : IInstallRepository
    {
        private readonly ApplicationDbContext _db;

        private readonly IWebHostEnvironment _env;

        private readonly ISignalRRepository signalRRepository;
        public InstallRepository(ApplicationDbContext db,IWebHostEnvironment env, ISignalRRepository signalRRepository)
        {
            _db = db;
            _env = env;
            this.signalRRepository = signalRRepository;
        }

        public async Task<List<InstallationCreateDto>> GetInstallationStatus(string userId)
        {
            try
            {
                var installations = await (from d in _db.Deliveries join t in _db.Technicians on d.AssignedToTechId equals t.TechnicianId
                                   join i in _db.Installations on d.DeliveryId equals i.DeliveryId join it in _db.InstallationTechnicians on i.InstallationId equals it.InstallationId
                                   where t.User.UserId == userId && d.AssignedToTechId == it.TechnicianId
                                   && (i.Status == InstallationStatus.Assigned.ToString()|| i.Status == InstallationStatus.InProgress.ToString())
                                    select new InstallationCreateDto
                                    {
                                        OrderId = d.OrderId,
                                        DeliveryId = d.DeliveryId,
                                        DeliveredDate = d.DeliveryAt??DateTime.Now,
                                        InstallationPrice = it.ServicePrice,
                                        InstallationId = i.InstallationId,
                                        InstallationStatus = i.Status
                                    }).OrderBy(x=>x.DeliveredDate)
                                    .ToListAsync();
                foreach (var installation in installations)
                {
                    if (installation.InstallationStatus == "InProgress")
                    {
                        installation.Actions = "Mark as Completed";
                    }

                    else if(installation.InstallationStatus == "Assigned")
                    {
                        installation.Actions = "InProgress";
                    }

                    else
                    {
                        installation.Actions = "Completed";
                    }
                }

                return installations;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task MarkasProgress(string id)
        {
            try
            {
                var reports = await _db.Set<Installations>().Include(x => x.installationTechnicians).ThenInclude(x => x.Technician).Where(x => x.InstallationId == id).FirstOrDefaultAsync();
                if (reports == null)
                {
                    throw new Exception(MessageHelper.Get("Installation", "NotFound"));
                }

                else
                {
                    reports.Status = reports.Status == InstallationStatus.Assigned.ToString() ?
                         InstallationStatus.InProgress.ToString() : reports.Status == InstallationStatus.InProgress.ToString() ?
                         InstallationStatus.Completed.ToString() : reports.Status;
                    await _db.SaveChangesAsync();

                    var messages = new Notifications
                    {
                        UserId = reports.installationTechnicians.First().Technician.UserId,
                        Message = "Updated status",
                        Type = NotificationType.System,
                        CreatedAt = TimeUtility.NowIST(),

                    };
                    await _db.Notifications.AddAsync(messages);
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<string> UploadInstallationReports(InstallationReportDto dto)
        {
            using var transcations = await _db.Database.BeginTransactionAsync();
            try
            {
                string userId = "";
                int orderId = 0;
                var installation = await _db.Set<Installations>().Include(x=>x.installationTechnicians).ThenInclude(x=>x.Technician).Include(x=>x.Order).ThenInclude(x=>x.Payment)
                    .Where(x => x.InstallationId == dto.InstallationId).FirstOrDefaultAsync();

                if (installation == null)
                {
                    throw new Exception(MessageHelper.Get("Installation", "NotFound"));
                }

              

                if (!dto.Images.Any() || dto.Images.Count == 0)
                {
                    throw new Exception(MessageHelper.Get("Installation", "None"));
                }

                var images = await UploadInstallationReportsImages(dto.Images);
                var imageUrls = JsonSerializer.Serialize(images);

                installation.Status = InstallationStatus.Completed.ToString();
                installation.CompletedDate = TimeUtility.NowIST();
                _db.Installations.Update(installation);


                var reports = new InstallationReports
                {
                    InstallationId = dto.InstallationId,
                    Notes = dto.Notes,
                    CheckList = dto.CheckList,
                    Image_Url = imageUrls,
                    CreatedAt=TimeUtility.NowIST(),
                    IsActive=true,
                };

                await _db.InstallationReports.AddAsync(reports);

                var order = installation.Order;

                if (order == null)
                {
                    throw new Exception("Order not found for installation");
                }

               
                var payment = await _db.Payments
                    .FirstOrDefaultAsync(x => x.OrderId == order.OrderId);

                if (payment != null && payment.PaymentMethod == PaymentMethod.COD.ToString())
                {
                    payment.PaymentStatus = PaymentStatus.Success.ToString();
                }

               
                order.Status = OrderStatus.DELIVERED;

                userId = order.UserId;
                orderId = order.OrderId;

                var messages = new Notifications
                {
                    UserId = userId,
                    Type = NotificationType.Installation,
                    CreatedAt = TimeUtility.NowIST(),
                    Message = $"your orderId {orderId} has successfully installed",
                    IsActive = false,
                    Title = "Installation Completed",
                };

                await _db.Notifications.AddAsync(messages);
                await _db.SaveChangesAsync();

                var signalRmsg = new NotificationDto
                {
                    UserId = messages.UserId,
                    NotificationId = messages.NotificationId,
                    NotificationType = messages.Type.ToString()!,
                    Title = messages.Title,
                    Message = messages.Message,
                    IsRead = false,
                    CreatedAt = messages.CreatedAt,

                };

                await signalRRepository.SendAsyncNotification(userId, signalRmsg);

                await transcations.CommitAsync();
               

                return $"{installation.installationTechnicians.First().Technician.UserId}";
            }
            catch (Exception)
            {
                await transcations.RollbackAsync();
                throw;
            }
        }


       

        private async Task<List<string>>UploadInstallationReportsImages(List<IFormFile> files)
        {
            try
            {

                if (files == null || files.Count == 0)
                    throw new Exception("No files uploaded");

                var uploadPaths = new List<string>();

                var folderPath = Path.Combine(_env.WebRootPath, "uploads", "installations");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                foreach (var file in files)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(folderPath, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    uploadPaths.Add($"/uploads/installations/{fileName}");
                }

                return uploadPaths;
            }


            catch (Exception)
            {
                throw;
            }
        }

        
        
    }
}
