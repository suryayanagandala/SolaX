using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.CustomMessages;
using CoreModels.Entities;
using CoreModels.Enums;
using DbAccessaLayer.Infrastructure;
using SolarXNUnit.SqLite.Base;
using SolarXNUnit.SqLite.SeedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarXNUnit.SqLite.Infrstructure
{
    public class NotificationTest:SqliteTestBase
    {
        private NotificationRepository noticationRepositry;
        private FakeSignalRRepository _fakeSignalRRepository;

        [SetUp]
        public void SetUp()
        {
            _fakeSignalRRepository = new FakeSignalRRepository();
            noticationRepositry = new NotificationRepository(_db,_fakeSignalRRepository);

        }

        [Test]
        public async Task AddNotifications_Success()
        {
            var dto = new Notifications
            {
               
                UserId = "ABX123",
                Message = "test db purpose",
                Type = NotificationType.System,
                IsActive = true,
                CreatedAt = TimeUtility.NowIST()
            };
            var result = await noticationRepositry.AddNotification(dto);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetListOfNotification_List_Sucess()
        {
            var userId = "ABX123";
            var result=await noticationRepositry.GetNotifiationDtos(userId);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetNotification_List_Invalid_Faliure()
        {
            string userId = "fakeId";
            var result = await noticationRepositry.GetNotifiationDtos(userId);
            Assert.That(result, Is.Empty);
        }

        //GetUnreadMessages
        

        [Test]
        public async Task GetUnreadMessages_Should_Return_List_When_Unread_Exists()
        {
        
            var userId = "USER19";

            _db.Notifications.Add(new Notifications
            {
                UserId = userId,
                Message = "Test Message",
                Title = "Test",
                Type = NotificationType.Performance,
                IsRead = false,
                CreatedAt = DateTimeOffset.UtcNow
            });

            await _db.SaveChangesAsync();

            
            var result = await noticationRepositry.GetUnreadMessages(userId);

            
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Test Message", result[0].Message);
        }

        [Test]
        public async Task GetUnreadMessages_Should_Return_Empty_List_When_No_Unread()
        {
        
            var userId = "USER20";

            _db.Notifications.Add(new Notifications
            {
                UserId = userId,
                Message = "Read Message",
                Title = "Test",
                Type = NotificationType.Delivery,
                IsRead = true,
                CreatedAt = DateTimeOffset.UtcNow
            });

            await _db.SaveChangesAsync();


            Assert.ThrowsAsync<NotFoundException>(async () => await noticationRepositry.GetUnreadMessages(userId));
        }




        [Test]
        public async Task GetUnreadMessages_Success_Case()
        {
            var dto = new Notifications
            {

                UserId = "ABX121",
                Message = "test db purpose",
                Type = NotificationType.System,
                IsActive = true,
                CreatedAt = TimeUtility.NowIST()
            };
            var result = await noticationRepositry.AddNotification(dto);
            _db.SaveChanges();



        }

        [Test]
        public void GetUnreadMessages_Should_Throw_Exception_When_Db_Fails()
        {
            
            var repository = new NotificationRepository(null,null); 

            Assert.ThrowsAsync<NullReferenceException>(async () =>
                await repository.GetUnreadMessages("USER12"));
        }


        [Test]
        public void GetUnreadMessages_Should_Throw_When_No_Unread()
        {
            var userId = "USER3";

            Assert.ThrowsAsync<NotFoundException>(async () =>
                await noticationRepositry.GetUnreadMessages(userId));
        }

        //MarkAssAllRead
        [Test]
        public async Task Mark_All_Read_Success()
        {
            string userId = "ABX123";
             await noticationRepositry.MarkAllAsRead(userId);
            var result=await noticationRepositry.GetNotifiationDtos(userId);
            Assert.That(result[0].IsRead, Is.True); 
        }

        //MarkAsRead
        [Test]
        public async Task MarkAsReadNotification_Succes()
        {
            int id = 1;
            await noticationRepositry.MarkAsRead(id);
            var updatedResult=_db.Notifications.Where(x => x.NotificationId == id).First();
            Assert.That(updatedResult.IsRead, Is.True);
        }

        [Test]
        public void MarkAsReadNotification_Faliure_CaseId()
        {
            int id = 3;

            Assert.ThrowsAsync<NotFoundException>(async () =>
                await noticationRepositry.MarkAsRead(id));
        }


        //GetCountOfUnreadNotifications
        [Test]
        public async Task GetCountOfUnreadNotifications_Success_Case()
        {
            string userId = "ABX123";
            int count=await noticationRepositry.GetCountOfUnreadNotifications(userId);
            Assert.That(count, Is.EqualTo(2));
        }
    }
}
