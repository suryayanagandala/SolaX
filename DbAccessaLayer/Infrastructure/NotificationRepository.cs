using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using CoreModels.Entities;
using DbAccessaLayer.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure
{
    public class NotificationRepository : INoticationRepositry
    {
        private readonly ApplicationDbContext _db;
        private readonly ISignalRRepository signalRRepository;
        public NotificationRepository(ApplicationDbContext db, ISignalRRepository signalRRepository)
        {
            _db = db;
            this.signalRRepository = signalRRepository;
        }
        public async Task<Notifications> AddNotification(Notifications notification)
        {
           await _db.Set<Notifications>().AddAsync(notification);
            await _db.SaveChangesAsync();
            return notification;
        }

      

        public async Task<List<NotificationDto>> GetNotifiationDtos(string userId)
        {
            try
            {
                var list= new List<NotificationDto>();

                var notifcations=await _db.Set<Notifications>().Where(x=>x.UserId==userId).OrderByDescending(x=>x.CreatedAt).ToListAsync();
                foreach(var item in notifcations)
                {
                    list.Add(new NotificationDto
                    {
                        UserId = userId,
                        NotificationType = item.Type.ToString()!,
                        Title=item.Title,
                        Message = item.Message,
                        IsRead = item.IsRead,
                        CreatedAt = item.CreatedAt,
                        NotificationId=item.NotificationId,
                    });
                }
                return list;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<List<NotificationDto>> GetUnreadMessages(string userId)
        {
            try
            {
                var notifications = await _db.Set<Notifications>().Where(x => !x.IsRead && x.UserId == userId).ToListAsync();

                if (!notifications.Any())
                {
                    throw new NotFoundException(MessageHelper.Get("Notifications", "NoMessages"));
                }

                var list=new List<NotificationDto>();    
                foreach(var item in notifications)
                {
                    list.Add(new NotificationDto
                    {
                        UserId=item.UserId,
                        Message=item.Message,
                        NotificationType=item.Type.ToString()!,
                        Title=item.Title,
                        CreatedAt=item.CreatedAt,

                    });
                }

                return list;

            }
            catch(Exception) { throw; }
        }

        public async Task MarkAllAsRead(string userId)
        {
            var notifications=await _db.Set<Notifications>().Where(x=>x.UserId==userId&&!x.IsRead).ToListAsync();
            foreach(var item in notifications)
            {
                item.IsRead = true;
           
            }
            
            await _db.SaveChangesAsync();
        }

        public async Task MarkAsRead(int notificationId)
        {
            try
            {
                var notification=await _db.Set<Notifications>().Where(x=>x.NotificationId==notificationId).FirstOrDefaultAsync();
                if (notification == null)
                {
                    throw new NotFoundException(MessageHelper.Get("Notifications", "NoMessages"));
                }

                else
                {
                    notification.IsRead=true;
                    _db.Set<Notifications>().Update(notification);
                    await _db.SaveChangesAsync();
                }
            }
            catch(Exception) { throw; }
        }

        public async Task<int> GetCountOfUnreadNotifications(string userId)
        {
            return await _db.Set<Notifications>().CountAsync(x=>x.UserId==userId&&!x.IsRead);
        }

        public async Task<List<string>> SendAlertNotificationsForAdmins(int panleId,string severity,double percentage)
        {
            try {
                var admins = await _db.Users.Include(x => x.Role).Where(x => x.Role.RoleName == "Admin").Select(x=>x.UserId).ToListAsync();
                foreach (var admin in admins)
                {
                    await SentAlertMessages(admin,panleId,severity,percentage);
                }
                return admins;
            }
            catch (Exception)
            {
                throw;
            }
        }


        private async Task SentAlertMessages(string userId,int panelId,string severity,double dropPercentage)
        {
            var Notification = new Notifications
            {
                UserId = userId,
                Message = $"{panelId} is in {severity} mode with {dropPercentage} %",
                Title = "Panel is in trouble",
                Type = CoreModels.Enums.NotificationType.System,
                CreatedAt = TimeUtility.NowIST()
            };
            await _db.Notifications.AddAsync(Notification);
            await _db.SaveChangesAsync();
                
           // var dto = new NotificationDto
           // {
           //     UserId = Notification.UserId,
           //     Message = Notification.Message,
           //     Title = Notification.Title,
           //     NotificationType = Notification.Type.ToString()!,
           //     CreatedAt = Notification.CreatedAt,
           //     IsRead = false,
           //     NotificationId = Notification.NotificationId,
           // };

           //await this.signalRRepository.SendNotificationAdmin(dto.UserId, dto);
        }


        
    }
}
