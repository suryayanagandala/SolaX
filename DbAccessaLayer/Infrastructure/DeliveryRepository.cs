using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using DbAccessaLayer.Helpers;
//using iText.Layout.Renderer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
//using MimeKit.Encodings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure
{
    public class DeliveryRepository : IDeliveryInterface
    {
        private readonly ApplicationDbContext _db;

        private readonly ISignalRRepository signalRRepository;

        public DeliveryRepository(ApplicationDbContext db, ISignalRRepository signalRRepository)
        {
            _db = db;
            this.signalRRepository = signalRRepository;
        }
        public async Task<string> MarkAsCompletedDelivery(int deliveryId)
        {
            using var transactions = await _db.Database.BeginTransactionAsync();
            try
            {
                
                var delivery = await _db.Set<Delivery>().Include(x=>x.AssignedToTech).Include(x=>x.Order).Where(x => x.DeliveryId == deliveryId&&x.Status== "OutForDelivery").FirstOrDefaultAsync();
                if (delivery == null)
                {
                    throw new Exception("No items are indelivery mode");
                }

                //if (delivery.Order == null)
                //    throw new Exception("delivery.Order is null");

                delivery.Status = "Delivered";
                if (delivery.AssignedToTechId != null && delivery.AssignedToTech!=null)
                {
                    delivery.AssignedToTech.IsActiveDeliveries -= 1;
                    delivery.AssignedToTech.IsAvailable = true;
                    delivery.DeliveryAt = DateTimeOffset.Now;
                    delivery.Order.Status = OrderStatus.DELIVERED;
                    _db.Deliveries.Update(delivery);
                   
                }

                var exists = await _db.Set<Installations>().AnyAsync(x => x.OrderId == delivery.OrderId);
                if (exists)
                {
                    throw new Exception(MessageHelper.Get("Installation","Installed"));
                }

                var installation = new Installations
                {
                    InstallationId = await GenerateInstallationId(),
                    OrderId=delivery.OrderId,
                    Status= InstallationStatus.Assigned.ToString(),
                    DeliveryId=deliveryId,
                    CreatedAt=TimeUtility.NowIST(),

                };
                await _db.Installations.AddAsync(installation);

                if(delivery.AssignedToTech != null&&delivery.AssignedToTechId!=null)
                {
                    await _db.InstallationTechnicians.AddAsync(new InstallationTechnician
                    {
                        InstallationId = installation.InstallationId,
                        TechnicianId = (int)delivery.AssignedToTechId,
                        ServicePrice = delivery.AssignedToTech.Service_Price,
                    });
                }

                var notification = new Notifications
                {
                    UserId = delivery.Order.UserId,
                    Type = NotificationType.Order,
                    Title = "Dlivery update",
                    IsActive = false,
                    Message = $"your orderId has succesfully delivered",
                    CreatedAt = TimeUtility.NowIST(),
                };

                await _db.Set<Notifications>().AddAsync(notification);
                await _db.SaveChangesAsync();

                var dto = new NotificationDto
                {
                    UserId = notification.UserId,
                    NotificationId = notification.NotificationId,
                    NotificationType = notification.Type.ToString()!,
                    Title = notification.Title,
                    Message = notification.Message,
                    IsRead = false,
                    CreatedAt = notification.CreatedAt,
                };

                await this.signalRRepository.SendAsyncNotification(notification.UserId,dto);
                await _db.SaveChangesAsync();
                await transactions.CommitAsync();
                return delivery.Order.UserId;
                

            }
            catch (Exception)
            {
                await transactions.RollbackAsync();
                throw;
            }
        }

        public async Task MarkAsOutOfDelivery(int deliveryId)
        {
            try
            {
                var delivery = await _db.Set<Delivery>().Include(x => x.Order).FirstOrDefaultAsync(x => x.DeliveryId == deliveryId);
                if (delivery == null)
                {
                    throw new Exception("No delivery found ");
                }
                if (delivery.Order == null)
                {
                    throw new Exception("Order not found for this delivery");
                }
                delivery.Status = "OutForDelivery";
                delivery.Order.Status = OrderStatus.SHIPPED;
                await _db.SaveChangesAsync();
            }
            catch (Exception)

            {
                throw;
            }
        }

        public async Task<List<string>> AssignedRemainingDeliveries()
        {
            var assignedUserIds = new List<string>();

            var deliveries = await _db.Deliveries.Where(x => x.AssignedToTechId == null && x.IsActive).ToListAsync();

            foreach (var item in deliveries)
            {
                var technician = await _db.Technicians.Where(x => x.IsAvailable).OrderBy(x => x.IsActiveDeliveries)
                                .FirstOrDefaultAsync();

                if (technician == null)
                    continue;

                item.AssignedToTechId = technician.TechnicianId;
                item.Status = "Assigned";
                technician.IsActiveDeliveries += 1;


                var notification = new Notifications
                {
                    UserId = technician.UserId,
                    Type = NotificationType.Delivery,
                    Title = "Delivery update",
                    IsActive = false,
                    Message = $"you have assigend new delivery",
                    CreatedAt=TimeUtility.NowIST(),
                };

                await _db.Set<Notifications>().AddAsync(notification);
                await _db.SaveChangesAsync();

                var dto = new NotificationDto
                {
                    UserId = notification.UserId,
                    NotificationId = notification.NotificationId,
                    NotificationType = notification.Type.ToString()!,
                    Title = notification.Title,
                    Message = notification.Message,
                    IsRead = false,
                    CreatedAt = notification.CreatedAt,
                };

                await this.signalRRepository.SendAsyncNotification(notification.UserId, dto);

                if (technician.IsActiveDeliveries == 5)
                {
                    technician.IsAvailable = false;
                }

                assignedUserIds.Add(technician.UserId);
            }

            await _db.SaveChangesAsync();

            return assignedUserIds.Distinct().ToList();
        }


        public async Task<List<DeliveryDto>> GetMarkoutDelieries(string userId)
        {
            try
            {
                var deliveries = await _db.Deliveries.Include(x => x.AssignedToTech).ThenInclude(x => x.User).Include(x => x.Order).ThenInclude(x => x.OrderItem)
                                 .Include(x => x.Order).ThenInclude(x => x.Payment).Where(x => x.AssignedToTechId != null && x.AssignedToTech!.UserId == userId && x.Status != "Delivered" && x.IsActive &&x.Order.Status!=OrderStatus.CANCELLED)
                                 .OrderBy(x=>x.AssignedAt).ToListAsync();

                if (!deliveries.Any())
                    throw new Exception("No deliveries found");

                var result = new List<DeliveryDto>();

                foreach (var item in deliveries)
                {
                   
                    var products = await (
                        from io in _db.OrderItems
                        join p in _db.Products on io.ProductId equals p.ProductId
                        join img in _db.ProductImages on p.ProductId equals img.ProductId
                        where io.OrderId == item.OrderId 
                        select new ProductImageForDelivery
                        {
                            ProductName = p.ProductName,
                            ImageUrl = img.ImageUrl
                        }
                    ).Distinct().ToListAsync();

                 
                    var payment = item.Order.Payment.FirstOrDefault(p => p.IsActive);

                    result.Add(new DeliveryDto
                    {
                        DeliveryId=item.DeliveryId,
                        OrderId = item.OrderId,
                        UserName = item.AssignedToTech!.User.FullName,
                        Products = products,
                        CustomerName=item.Order.FullName,
                        DelievryAddress=item.Order.DeliveryAddress,
                        PhoneNumber=item.Order.PhoneNumber,
                        Quantity = item.Order.OrderItem.Sum(x => x.Quantity),
                        PaymentMethod = payment?.PaymentMethod ?? "UnKnown",
                        Price = payment?.Amount ?? 0,
                        Status = item.Status!,
                        DeliveryAt=item.DeliveryAt??DateTimeOffset.Now,
                    });
                }

                return result;


            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<string> GenerateInstallationId()
        {
            var today = DateTimeOffset.Now.Date;
            var tomorrow = today.AddDays(1);

            var countToday = await _db.Installations
                .CountAsync(x => x.CreatedAt >= today && x.CreatedAt < tomorrow);

            return $"INST-{today:yyyyMMdd}-{(countToday + 1).ToString("D4")}";
        }

        public async Task<List<DeliveryDto>> GetDeliveredOrders(string userId)
        {
            try
            { 
                var deliveries=await (from d in _db.Deliveries join t in _db.Technicians on d.AssignedToTechId equals
                          t.TechnicianId join u in _db.Users on t.UserId equals u.UserId
                          where u.UserId == userId && d.Status== "Delivered"
                          select new DeliveryDto
                          {
                              UserId=userId,
                              DeliveryAt=d.DeliveryAt??DateTimeOffset.Now,
                              DelievryAddress=d.Order.DeliveryAddress,
                              PaymentMethod=d.Order.Payment.First().PaymentMethod,
                              Status=d.Status!,
                              OrderId=d.OrderId
                          }
                          ).ToListAsync();
                return deliveries.OrderByDescending(x => x.DeliveryAt).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SentDelayMessagesForDeliveryData()
        {
            var todayData = TimeUtility.NowIST();
            var lateDeliveries=await _db.Deliveries.Include(x=>x.Installations).Include(x=>x.Order).Where(x=>x.Status!=DeliveryStatus.Delivered.ToString()&&
            x.DeliveryAt<todayData).ToListAsync();

            foreach (var delivery in lateDeliveries)
            {
                var notification = new Notifications
                {
                    UserId = delivery.Order.UserId,
                    Type = NotificationType.Delivery,
                    Title = "Delivery update",
                    IsActive = false,
                    Message = $"Sorry for delay your delivery is reaching at {todayData.AddDays(1)}",
                    CreatedAt = TimeUtility.NowIST(),
                };

                await _db.Set<Notifications>().AddAsync(notification);
                await _db.SaveChangesAsync();

                var dto = new NotificationDto
                {
                    UserId = notification.UserId,
                    NotificationId = notification.NotificationId,
                    NotificationType = notification.Type.ToString()!,
                    Title = notification.Title,
                    Message = notification.Message,
                    IsRead = false,
                    CreatedAt = notification.CreatedAt,
                };

                await signalRRepository.SendAsyncNotification(notification.UserId, dto);

                if (delivery.AssignedToTechId.HasValue && delivery.AssignedToTech!=null)
                {
                    var techNotifications = new Notifications
                    {
                        UserId = delivery.AssignedToTech.UserId,
                        Type = NotificationType.Delivery,
                        Title = "Delivery update",
                        IsActive = false,
                        Message = $"Hurry up the orderId {delivery.OrderId} is still pending please try to completed at  {todayData.AddDays(1)}",
                        CreatedAt = TimeUtility.NowIST(),
                    };

                    await _db.Set<Notifications>().AddAsync(techNotifications);
                    await _db.SaveChangesAsync();

                    var dtos = new NotificationDto
                    {
                        UserId = techNotifications.UserId,
                        NotificationId = techNotifications.NotificationId,
                        NotificationType = techNotifications.Type.ToString()!,
                        Title =techNotifications.Title,
                        Message = techNotifications.Message,
                        IsRead = false,
                        CreatedAt = techNotifications.CreatedAt,
                    };

                    await signalRRepository.SendAsyncNotification(techNotifications.UserId, dtos);
                }
                ;

            };
        }
    }
}
