using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using DbAccessaLayer.Helpers;
using iText.Bouncycastle.Crypto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure
{
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly ApplicationDbContext _db;

        private readonly IWebHostEnvironment web;

        private readonly ISignalRRepository signalRRepository;
        public MaintenanceRepository(ApplicationDbContext db, IWebHostEnvironment web, ISignalRRepository signalRRepository)
        {
            _db = db;
            this.web = web;
            this.signalRRepository = signalRRepository;
        }
        public List<DeliveryDto> GetMaintenanceStatus(string userId)
        {
            var items = (from ir in _db.Set<InstallationReports>()
                         join i in _db.Set<Installations>() on ir.InstallationId equals i.InstallationId
                         join d in _db.Set<Delivery>() on i.DeliveryId equals d.DeliveryId
                         join o in _db.Set<OrderItems>() on d.OrderId equals o.OrderId
                         where d.Order.UserId == userId
                         && d.Status == DeliveryStatus.Delivered.ToString()
                         && i.Status == InstallationStatus.Completed.ToString()

                         group new { o, d, i } by new
                         {
                             d.OrderId,
                             o.ProductId,
                             o.Product.ProductName,
                             o.Product.ProductImages.First().ImageUrl,
                             d.DeliveryAt,
                             i.Status,
                             i.IsActive,
                             i.InstallationId
                         } into g

                         select new DeliveryDto
                         {
                             OrderId = g.Key.OrderId,
                             ProductName = g.Key.ProductName,
                             ProductImage = g.Key.ImageUrl,
                             DeliveryAt = g.Key.DeliveryAt ?? DateTime.Now,
                             Status = g.Key.Status,
                             IsActive = g.Key.IsActive,
                             InstallationId = g.Key.InstallationId,
                             Quantity = g.Count(),   
                             Price = (decimal)g.Sum(x => (double)x.o.UnitPrice)
                         }).ToList();

            return items;
        }

        public async Task RaiseComplete(MaintenanceRequestCreateDto dto)
        {
            using var transactions = await _db.Database.BeginTransactionAsync();
            try
            {
                var installation = await _db.Installations.Include(x=>x.MaintenanceRequest).Include(x => x.delivery).ThenInclude(x => x.Order).ThenInclude(x=>x.User).Where(x =>x.InstallationId == dto.InstallationId &&x.Status == InstallationStatus.Completed.ToString() &&
                                       x.delivery.Status == DeliveryStatus.Delivered.ToString() &&x.delivery.Order.User.UserId == dto.UserId).FirstOrDefaultAsync();


                if (installation == null)
                    throw new NotFoundException(MessageHelper.Get("Installation", "NotFound"));

                if (!installation.IsActive)
                    throw new DataExistsException(MessageHelper.Get("Maintenance","Exist"));
                installation.IsActive = false;

              

                var requestId = await GenerateRequestId();

                var request = new MaintenanceRequests
                {
                    RequestId = requestId,
                    InstallationId = dto.InstallationId,
                    Status = MaintenanceStatus.Pending.ToString(),
                    Description = dto.Description,
                    UserId = dto.UserId,
                    CreatedAt=TimeUtility.NowIST()
                };
                await _db.Set<MaintenanceRequests>().AddAsync(request);

                await AssignMaintenance(request);

                var messages = new Notifications
                {
                    UserId = dto.UserId,
                    Type = NotificationType.Maintenance,
                    Title = "Raise a request",
                    Message = $"Your installationId {dto.InstallationId} has been sent request successfully",
                    CreatedAt = TimeUtility.NowIST()
                };

                await _db.Notifications.AddAsync(messages);

                var signal = new NotificationDto
                {

                    UserId = dto.UserId,
                    NotificationId = messages.NotificationId,
                    NotificationType = messages.Type.ToString()!,
                    Title = messages.Title,
                    Message = messages.Message,
                    IsRead = false,
                    CreatedAt = messages.CreatedAt,

                };

                await signalRRepository.SendAsyncNotification(dto.UserId, signal);
                await _db.SaveChangesAsync();
                await transactions.CommitAsync();


            }
            catch (Exception)
            {
                await transactions.RollbackAsync();
                throw;
            }
        }



        private Task<string> GenerateRequestId()
        {
            var random = new Random();

            return Task.FromResult($"R{random.Next(1000000, 9999999)}");
        }



        private async Task AssignMaintenance(MaintenanceRequests request)
        {
            var technician = await _db.Technicians
                .Where(x =>
                    x.Specialization != TechnicianRoles.Installer.ToString() &&
                    x.IsAvailable &&
                    x.IsActiveDeliveries < 5 &&
                    x.IsActive)
                .OrderBy(x => x.IsActiveDeliveries)
                .ThenBy(x => x.TechnicianId)
                .FirstOrDefaultAsync();

            if (technician == null)
                return; 

            var maintenanceTechnician = new MaintenanceTechnicians
            {
                RequestId = request.RequestId,
                TechnicianId = technician.TechnicianId,
                ServicePrice = technician.Service_Price
            };

            await _db.MaintenanceTechnicians.AddAsync(maintenanceTechnician);

            
            request.Status = MaintenanceStatus.Assigned.ToString();

      
            technician.IsActiveDeliveries++;

            if (technician.IsActiveDeliveries >= 5)
                technician.IsAvailable = false;

            await _db.SaveChangesAsync();
        }


        public async Task<List<MaintenanceRequestDto>> maintenanceRequestDtos(string userId)
        {
            
                var assignedItems = await (from m in _db.Set<MaintenanceRequests>()
                                     join i in _db.Set<Installations>() on m.InstallationId equals i.InstallationId
                                     join or in _db.Set<OrderItems>() on i.OrderId equals or.OrderId
                                     join o in _db.Set<Orders>() on or.OrderId equals o.OrderId
                                     join mt in _db.Set<MaintenanceTechnicians>() on m.RequestId equals mt.RequestId
                                     join t in _db.Set<Technicians>()
                                     on mt.TechnicianId equals t.TechnicianId
                                     where t.UserId == userId && m.IsActive && i.IsActive == false && m.Status != MaintenanceStatus.Completed.ToString()
                                     select new MaintenanceRequestDto
                                     {
                                         UserId = userId,
                                         OrderId = or.OrderId,
                                         CustomerAddress = o.DeliveryAddress,
                                         CustomerPhoneNumber = o.PhoneNumber,
                                         Status = m.Status,
                                         AssignedDate = m.CreatedAt,
                                         RequestId=m.RequestId,

                                     }).ToListAsync();
            ;

                return  assignedItems;                                
                                   
          
        }

        public async Task<List<string>>UploadMaintenanceImages(List<IFormFile> files)
        {
            try
            {
                if (files.Count == 0 || !files.Any())
                {

                    throw new Exception("Please uplaod ateast ome image");
                }

                var uploadPath = new List<string>();

                var folderPath = Path.Combine(web.WebRootPath, "uploads", "maintenance");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                foreach ( var file in files)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(folderPath, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    uploadPath.Add($"/uploads/installations/{fileName}");
                }
                return uploadPath;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<string>> CompletedMaintenanceTask(MaintenanceReportDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.RequestId))
                throw new ValidationException("RequestId is required.");

            if (dto.Images == null || !dto.Images.Any())
                throw new ValidationException("At least one image must be uploaded.");

            var request = await _db.Set<MaintenanceRequests>()
                .Include(x => x.Maintenances)
                    .ThenInclude(x => x.Technician)
                .Include(x => x.Installation)
                .FirstOrDefaultAsync(x => x.RequestId == dto.RequestId);

            if (request == null)
                throw new NotFoundException(MessageHelper.Get("Maintenance", "NotFound"));

            if (request.Status == MaintenanceStatus.Completed.ToString())
                throw new DataExistsException(MessageHelper.Get("Maintenance","Completed"));

           
            var uploadedImages = await UploadMaintenanceImages(dto.Images);
            var imageUrlsJson = JsonSerializer.Serialize(uploadedImages);

         
            request.Status = MaintenanceStatus.Completed.ToString();
            
            request.IsActive = false;

            if (request.Installation != null)
                request.Installation.IsActive = true;

          
            foreach (var maintenance in request.Maintenances)
            {
                var technician = maintenance.Technician;

                if (technician != null && technician.IsActiveDeliveries > 0)
                {
                    technician.IsActiveDeliveries -= 1;
                }
            }

          
            var report = new MaintenanceReports
            {
                RequestId = request.RequestId,
                ReplacedParts = dto.ReplacedParts,
                Notes = dto.Notes,
                Image_Url = imageUrlsJson,
                CreatedAt=TimeUtility.NowIST()
            };

            await _db.Set<MaintenanceReports>().AddAsync(report);

            await _db.SaveChangesAsync();

           
            var result = new List<string>();

            var firstTechnician = request.Maintenances.FirstOrDefault()?.Technician;

            if (firstTechnician != null)
                result.Add(firstTechnician.UserId.ToString());

            if (request.UserId != null)
                result.Add(request.UserId.ToString());

            if (request.Installation?.OrderId != null)
                result.Add(request.Installation.OrderId.ToString());

            result.Add(request.RequestId);

            return result;
        }

        public async Task<List<object>> GetCompletedMaintenances(string userId)
        {
            
                var maintenances = await (from m in _db.Set<MaintenanceRequests>() join mt in _db.Set<MaintenanceTechnicians>()
                                       on m.RequestId equals mt.RequestId join t in _db.Set<Technicians>() on mt.TechnicianId equals t.TechnicianId
                                          join mr in _db.Set<MaintenanceReports>() on mt.RequestId equals mr.RequestId join i in _db.Set<Installations>()
                                          on m.InstallationId equals i.InstallationId 
                                          join d in _db.Set<Delivery>() on i.DeliveryId equals d.DeliveryId join o in _db.Set<Orders>()
                                          on d.OrderId equals o.OrderId
                                          where m.Status == MaintenanceStatus.Completed.ToString() && t.UserId == userId && t.IsActive select new
                                          {
                                              RequestId=mt.RequestId,
                                              CompleteDate=mr.CreatedAt,
                                              Address=o.DeliveryAddress,
                                              Status=i.Status,

                                          }

                                       ).OrderByDescending(x=>x.CompleteDate).ToListAsync<object>();
                return maintenances;
        
          
        }

        public async Task SendNotificationsForDelayMaintenance()
        {
            try
            {
                var today = TimeUtility.NowIST();
                var pendingMaintenances = await _db.Set<MaintenanceRequests>().Include(x=>x.Maintenances).Where(x => x.IsActive&&x.CreatedAt.AddDays(5)<today).ToListAsync();
                foreach ( var pendingMaintenance in pendingMaintenances)
                {
                    var notifications = new Notifications
                    {
                        UserId = pendingMaintenance.UserId,
                        Message="Your Maintenace request is pending please wait for some time",
                        Type=NotificationType.Maintenance,
                        Title="Maintenance Delay",
                        CreatedAt=TimeUtility.NowIST(),

                    };
                    await _db.AddAsync(notifications);

                    var signal = new NotificationDto
                    {

                        UserId = notifications.UserId,
                        NotificationId = notifications.NotificationId,
                        NotificationType = notifications.Type.ToString()!,
                        Title = notifications.Title,
                        Message = notifications.Message,
                        IsRead = false,
                        CreatedAt = notifications.CreatedAt,

                    };

                    await signalRRepository.SendAsyncNotification(notifications.UserId, signal);

                    var assignedMaintenaces = await (from mt in _db.Set<MaintenanceTechnicians>()
                                                     join m in _db.Set<MaintenanceRequests>()
                                                  on mt.RequestId equals m.RequestId
                                                     join t in _db.Set<Technicians>() on mt.TechnicianId equals t.TechnicianId
                                                     where m.Status == MaintenanceStatus.Assigned.ToString() && m.UserId == pendingMaintenance.UserId
                                                     select new
                                                     {
                                                         userId = t.UserId,
                                                     }
                                                     ).ToListAsync();
                    foreach( var m in assignedMaintenaces)
                    {
                        var dto = new Notifications
                        {
                            UserId = m.userId,
                            Message = "Your Maintenace request is pending please wait for some time",
                            Type = NotificationType.Maintenance,
                            Title = "Maintenance Delay",
                            CreatedAt = TimeUtility.NowIST(),

                        };


                        await _db.AddAsync(dto);

                        var signals = new NotificationDto
                        {

                            UserId = notifications.UserId,
                            NotificationId = notifications.NotificationId,
                            NotificationType = notifications.Type.ToString()!,
                            Title = notifications.Title,
                            Message = notifications.Message,
                            IsRead = false,
                            CreatedAt = notifications.CreatedAt,

                        };

                        await signalRRepository.SendAsyncNotification(notifications.UserId, signals);


                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
