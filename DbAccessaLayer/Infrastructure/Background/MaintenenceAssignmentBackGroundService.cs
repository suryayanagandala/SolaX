using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure.Background
{
    public class MaintenenceAssignmentBackGroundService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        

        public MaintenenceAssignmentBackGroundService(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;

          
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var signalRepository = scope.ServiceProvider.GetRequiredService<ISignalRRepository>();
                    var pendingRequests = await _db.MaintenanceRequests
                        .Where(x => x.Status == MaintenanceStatus.Pending.ToString())
                        .OrderBy(x => x.CreatedAt)
                        .ToListAsync(stoppingToken);


                    foreach (var request in pendingRequests)
                    {
                       await using var transaction = await _db.Database.BeginTransactionAsync(stoppingToken);
                        try
                        {
                            var technician = await _db.Technicians.Where(x => x.IsAvailable == true && x.IsActiveDeliveries < 5 && x.IsActive == true).OrderBy(x => x.IsActiveDeliveries).ThenBy(x => x.TechnicianId).FirstOrDefaultAsync(stoppingToken);

                            if (technician == null)
                                continue;

                            else
                            {
                                var maintenanceTechnician = new MaintenanceTechnicians
                                {
                                    RequestId = request.RequestId,
                                    TechnicianId = technician.TechnicianId,
                                    ServicePrice = technician.Service_Price,
                                };

                                await _db.MaintenanceTechnicians.AddAsync(maintenanceTechnician, stoppingToken);

                                request.Status = MaintenanceStatus.Assigned.ToString();

                                technician.IsActiveDeliveries += 1;

                                if (technician.IsActiveDeliveries >= 5)
                                    technician.IsAvailable = false;
                                var messages = new Notifications
                                {
                                    UserId = technician.UserId,
                                    Type = NotificationType.Maintenance,
                                    Title = "New Work",
                                    Message = $"New Maintenance as been assigned to you by admin"
                                };
                                await _db.Notifications.AddAsync(messages, stoppingToken);

                                var dto = new NotificationDto
                                {
                                    UserId = technician.UserId,
                                    NotificationId = messages.NotificationId,
                                    NotificationType = messages.Type.ToString()!,
                                    Title = messages.Title,
                                    Message = messages.Message,
                                    IsRead = false,
                                    CreatedAt = messages.CreatedAt,
                                };

                                await _db.SaveChangesAsync(stoppingToken);
                                await transaction.CommitAsync(stoppingToken);

                                await signalRepository.SendAsyncNotification(dto.UserId,dto);


                            }
                            
                        }
                        catch (Exception)
                        {
                            await transaction.RollbackAsync(stoppingToken);
                            throw;
                        }


                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex); 
                }

               
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

    }
}
