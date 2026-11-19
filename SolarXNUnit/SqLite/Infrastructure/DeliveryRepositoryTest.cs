using CoreModels;
using CoreModels.Entities;
using CoreModels.Enums;
using DbAccessaLayer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SolarXNUnit.SqLite.Base;
using SolarXNUnit.SqLite.SeedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarXNUnit.SqLite.Infrstructure
{
    public class DeliveryRepositoryTest:SqliteTestBase
    {
        private  DeliveryRepository deliveryRepository;
        private  FakeSignalRRepository fakeSignalRRepository;

        [SetUp]
        public void setUp()
        {
            fakeSignalRRepository = new FakeSignalRRepository();
            deliveryRepository = new DeliveryRepository(_db, fakeSignalRRepository);
        }

        [Test]
        public async Task MarkAsCompletedDelivery_Should_AlreadyInstallationCreated()
        {
          
            var delivery = _db.Deliveries.First(d => d.Status == "OutForDelivery");
            delivery.AssignedToTechId = 1;
            delivery.AssignedToTech = _db.Technicians.First();
            await _db.SaveChangesAsync();

          

            // Assert
            Assert.ThrowsAsync<Exception>(async () => await deliveryRepository.MarkAsCompletedDelivery(delivery.DeliveryId));

            //Assert.IsTrue(fakeSignalRRepository.SentNotifications.Any());
            //var sentNotification = fakeSignalRRepository.SentNotifications.First();
            //Assert.AreEqual(order.UserId, sentNotification.UserId);
        }

        [Test]
        public async Task MarkAsCompletedDelivery_Should_UpdateStatusAndCreateInstallation()
        {
            try
            {
                var user = new Users
                {
                    UserId = "ERCXEk",
                    FullName = "Suryaq",
                    EmailId = "surya@12gmail.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Surya@2003"),
                    PhoneNumber = "9876543213",
                    IsActive = true,
                    Address = "jfiurhurhf",
                    CreatedBy = "Admin",

                };
                await _db.Users.AddAsync(user);

                // Technician
                var tech = new Technicians
                {
                    TechnicianId = 3,
                    UserId = user.UserId,
                    IsAvailable = true,
                    IsActiveDeliveries = 0,
                    Specialization = "Installer",
                    Qualification = "Diploma",
                    Service_Price = 500
                };
                await _db.Technicians.AddAsync(tech);

                // Order
                var order = new Orders
                {
                    OrderId = 99,
                    UserId = user.UserId,
                    Status = OrderStatus.CREATED,
                    TotalAmount = 15000,
                    PhoneNumber = user.PhoneNumber,
                    FullName = user.FullName,
                    IsActive = true,
                    DeliveryAddress = "Vijayawada Kondapalli",
                    CreatedAt = TimeUtility.NowIST(),
                    OrderItem = new List<OrderItems>
                {
                    new OrderItems
                    {
                        OrderItemId = 99,
                        ProductId = 1,
                        Quantity = 1,
                        UnitPrice = 15000
                    }
                }
                };
                await _db.Orders.AddAsync(order);

                // Delivery
                var delivery = new Delivery
                {
                    DeliveryId = 99,
                    OrderId = order.OrderId,
                    AssignedToTechId = tech.TechnicianId,
                    Status = DeliveryStatus.OutForDelivery.ToString(),
                    AssignedAt = TimeUtility.NowIST(),
                    DeliveryAt = TimeUtility.NowIST().AddDays(1),
                    AssignedBy = "Admin",
                    AssignedToTech = tech,  
                    Order = order          
                };
                await _db.Deliveries.AddAsync(delivery);

                await _db.SaveChangesAsync();

                var userId = await deliveryRepository.MarkAsCompletedDelivery(99);

                // Assert
                var updatedDelivery = await _db.Deliveries
                    .Include(d => d.Order)
                    .FirstOrDefaultAsync(d => d.DeliveryId == 99);

                Assert.Multiple(() =>
                {
                    Assert.AreEqual("Delivered", updatedDelivery.Status);
                    Assert.AreEqual(OrderStatus.DELIVERED, updatedDelivery.Order.Status);
                    Assert.True(fakeSignalRRepository.WasCalled);
                    Assert.That(_db.Notifications.Count(), Is.EqualTo(3));
                    Assert.That(_db.Installations.Count(), Is.EqualTo(5));
                });



            }
            catch (DbUpdateException ex)
            {
                var errorMessages=ex.InnerException!=null? ex.InnerException.Message:ex.Message;
                Console.WriteLine(errorMessages);
                throw new Exception(errorMessages);

            }
            
        }


        [Test]
        public void MarkAsCompletedDelivery_DeliveryNotFound_ThrowsException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await deliveryRepository.MarkAsCompletedDelivery(199));

            Assert.That(ex.Message, Is.EqualTo("No items are indelivery mode"));
        }


        [Test]
        public async Task MarkAsCompletedDelivery_DeliveryOrderNull_ThrowsException()
        {
            // Arrange
            var delivery = new Delivery
            {
                DeliveryId = 5,
                Status = "OutForDelivery",
                AssignedToTechId = null,
                AssignedToTech = null,
                AssignedAt=TimeUtility.NowIST(),
                OrderId =5,
            };
            _db.Deliveries.Add(delivery);
            await _db.SaveChangesAsync();

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await deliveryRepository.MarkAsCompletedDelivery(delivery.DeliveryId));

            
        }

        [Test]
        public void GetDeliveies()
        {
            string userId = "ABX123";
            var result=deliveryRepository.GetMarkoutDelieries(userId);
            Assert.That(result, Is.Not.Null);
        }


        [Test]
        public void GetDeliveries_fAkeId()
        {
            string userId = "fakeId";
            Assert.ThrowsAsync<Exception>(async () => await deliveryRepository.GetMarkoutDelieries(userId));
        }

        [Test]
        public void GetDeliveredOrders()
        {
            var userId = "ABX123";
            var result=deliveryRepository.GetDeliveredOrders(userId);
            Assert.That(result,Is.Not.Null);
        }

        [Test]
        public async Task GetDeliveredOrders_Falire()
        {
            var userId = "fakeId";
            Assert.IsEmpty(await deliveryRepository.GetDeliveredOrders(userId));
        }


        [Test]
        public async Task MarkAsOutOfDelivery_Success()

        { 
            var dto = new Delivery()
            {
                DeliveryId = 4,
                OrderId = 1,
                AssignedAt = TimeUtility.NowIST(),
                AssignedToTechId = 1,
                DeliveryAt = TimeUtility.NowIST().AddDays(2),
            };
            _db.Deliveries.Add(dto);
            _db.SaveChanges();
            var deliveryId = dto.DeliveryId;
            await deliveryRepository.MarkAsOutOfDelivery(deliveryId);
            var updatedData = await _db.Deliveries.FirstOrDefaultAsync(x=>x.DeliveryId==dto.DeliveryId);
            if (updatedData!=null)
            {
                Assert.That(updatedData.Status, Is.EqualTo(DeliveryStatus.OutForDelivery.ToString()));
            }

        }

        [Test]
        public void MarkAsOutOfDelivery_Failure()
        {
            int deliveryId = 80;
            var ex=Assert.ThrowsAsync<Exception>(async()=>await deliveryRepository.MarkAsOutOfDelivery(deliveryId));
            Assert.That(ex.Message, Does.StartWith("No delivery found "));
        }



        //AssignedReaminingDeliveries
        [Test]
        public async Task AssignedRemainingDeliveries_NoTechnicianAvailable_ShouldNotAssign()
        {
            var delivery = new Delivery
            {
                DeliveryId = 6,
                AssignedToTechId = null,
                IsActive = true,
                OrderId=2,
                AssignedAt=TimeUtility.NowIST(),
            };

            await _db.Deliveries.AddAsync(delivery);
            await _db.SaveChangesAsync();

            foreach (var technician in _db.Technicians) technician.IsAvailable = false;
            await _db.SaveChangesAsync();
            var result = await deliveryRepository.AssignedRemainingDeliveries();

            var updatedDelivery = await _db.Deliveries.FirstAsync(x => x.DeliveryId == 6);

            var updatedTechnician = await _db.Technicians.Where(x => x.Specialization == TechnicianRoles.Installer.ToString())
                                  .FirstAsync();

            Assert.Multiple(() =>
            {
                Assert.That(updatedDelivery.AssignedToTechId, Is.Null);
                Assert.That(result.Count, Is.EqualTo(0));
                Assert.That(updatedTechnician.IsActiveDeliveries, Is.EqualTo(0));
                Assert.That(updatedDelivery.Status, Is.Null);
                Assert.False(fakeSignalRRepository.WasCalled);
            });
        }

        [Test]
        public async Task SendDeliveryDelayMeesages_Success()
        {
            var dto = new Delivery
            {
                OrderId = 2,
                DeliveryId = 4,
                AssignedToTechId = 1,
                AssignedAt = TimeUtility.NowIST().AddDays(-4),
                DeliveryAt = TimeUtility.NowIST().AddDays(-1),
                AssignedBy = "Admin",
                Status = DeliveryStatus.Pending.ToString()
            };

            await _db.Deliveries.AddAsync(dto);
            await _db.SaveChangesAsync();

            await deliveryRepository.SentDelayMessagesForDeliveryData();
            var notification = await _db.Notifications.Where(x => x.UserId == "ABX123").OrderByDescending(x => x.NotificationId)
                               .FirstAsync();

            var technicianNotifications = await _db.Notifications.Where(x => x.UserId == "ABX123").OrderByDescending(x => x.NotificationId).FirstAsync();
            Assert.Multiple(() =>
            {
                Assert.True(fakeSignalRRepository.WasCalled);
                Assert.That(_db.Notifications.Count(), Is.EqualTo(4));
                Assert.That(notification.Type,Is.EqualTo(NotificationType.Delivery));
                Assert.That(technicianNotifications.Message, Does.Contain("Hurry up the orderId"));
            });
           
        }
    }

  
}
