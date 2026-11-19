using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
using CoreModels.Enums;
using DbAccessaLayer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SolarXNUnit.SqLite.Base;
using SolarXNUnit.SqLite.SeedData;
using System.Runtime.InteropServices;

namespace SolarXNUnit.SqLite.Tests
{
    public class PaymentRepositoryTests : SqliteTestBase
    {
        private PaymentRepository _repository;
        private FakeSignalRRepository _fakeSignalRRepository;   

        [SetUp]
        public void SetupRepository()
        {
            var options = Options.Create(new AssignmentSettings
            {
                AssignedDelivery = 2
            });

            _fakeSignalRRepository = new FakeSignalRRepository();
            _repository = new PaymentRepository(
                _db,
                options,
                _fakeSignalRRepository
            );
        }

        [Test]
        public async Task PaymentGateway_Should_Process_COD_Successfully()
        {
            

            var order = await _db.Orders.Include(x => x.Payment).FirstOrDefaultAsync();

           
            if (order.Payment.Any())
            {
                _db.Payments.RemoveRange(order.Payment);
                await _db.SaveChangesAsync();
            }

            var dto = new PaymentCreateDto
            {
                OrderId = order.OrderId,
                PaymentMethod = "COD"
            };

            // Act
            var result = await _repository.PaymentGateway(dto);

            // Assert
            Assert.IsNotNull(result);

            var payment = await _db.Payments
                .FirstOrDefaultAsync(p => p.OrderId == order.OrderId);

            Assert.IsNotNull(payment);
            Assert.AreEqual("COD", payment.PaymentMethod);
            Assert.AreEqual("PENDING", payment.PaymentStatus);

            var updatedOrder = await _db.Orders.FirstAsync(x => x.OrderId == order.OrderId);

            Assert.AreEqual(OrderStatus.CONFIRMED, updatedOrder.Status);

            var delivery = await _db.Deliveries.FirstOrDefaultAsync(x => x.OrderId == order.OrderId);

            Assert.IsNotNull(delivery);

            var invoice = await _db.Invoices.FirstOrDefaultAsync(x => x.OrderId == order.OrderId);

            Assert.IsNotNull(invoice);

            Assert.That(_db.Notifications.Count(), Is.EqualTo(4));

            Assert.IsTrue(_fakeSignalRRepository.WasCalled);


            Assert.IsNotEmpty(result.UserId);
        }

        [Test]
        public void PaymentGateway_Should_Throw_When_OrderNotFound()
        {
            var dto = new PaymentCreateDto
            {
                UserId = "ERCXET",
                OrderId = 999,
                PaymentMethod = "COD",

            };
            Assert.ThrowsAsync<Exception>(async()=>await _repository.PaymentGateway(dto));

        }

        [Test]
        public async Task PaymentGateway_Should_Throw_When_AlreadyPaid()
        {
            // Remove the fixture payment so the first call can pay this order.
            var seededPayment = await _db.Payments.Where(x => x.OrderId == 1).ToListAsync();
            _db.Payments.RemoveRange(seededPayment);
            await _db.SaveChangesAsync();

            // Arrange: pay once
            var dto1 = new PaymentCreateDto
            {
                OrderId = 1,
                PaymentMethod = "ONLINE",
                UserId = "ERCXET"
            };
            await _repository.PaymentGateway(dto1);

            // Act & Assert: pay again should throw
            var dto2 = new PaymentCreateDto
            {
                OrderId = 1,
                PaymentMethod = "COD",
                UserId = "ERCXET"
            };

            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.PaymentGateway(dto2));

            Assert.That(ex.Message, Is.EqualTo("Already payment exist"));
        }

        [Test]
        public async Task PaymentGateway_Should_Process_Online_Successfully()
        {
            var dto = new PaymentCreateDto
            {
                OrderId = 2,
                PaymentMethod = "ONLINE",
                UserId= "ABX123"
            };

            var result = await _repository.PaymentGateway(dto);

            Assert.AreEqual(2, result.OrderId);
            Assert.AreEqual("ONLINE", result.PaymentMethod);
            

            var payment = await _db.Payments.FirstOrDefaultAsync(p => p.OrderId == dto.OrderId);
            Assert.IsNotNull(payment);
            Assert.AreEqual("SUCCESS", payment.PaymentStatus);

            var delivery = await _db.Deliveries.FirstOrDefaultAsync(d => d.OrderId == dto.OrderId);
            Assert.IsNotNull(delivery);
            Assert.AreEqual("Assigned", delivery.Status);

            Assert.IsTrue(_fakeSignalRRepository.WasCalled);
        }

        [Test]
        public async Task AssignDeliveries_Should_Handle_NoAvailableTechnician()
        {
            // Arrange: make all technicians unavailable
            foreach (var tech in _db.Technicians)
            {
                tech.IsAvailable = false;
            }
            await _db.SaveChangesAsync();

            var dto = new PaymentCreateDto
            {
                OrderId = 2,
                PaymentMethod = "COD",
                UserId= "ABX123"
            };

            // Act
            var result = await _repository.PaymentGateway(dto);

            // Assert: delivery is still created but status is Pending
            var delivery = await _db.Deliveries.FirstOrDefaultAsync(d => d.OrderId == dto.OrderId);
            Assert.IsNotNull(delivery);
            Assert.AreEqual("Pending", delivery.Status);
            Assert.IsNotNull(delivery.DeliveryAt);
        }

        [Test]
        public void GenarateQrCode()
        {
            var result = _repository.GenerateQr(1);
            Assert.IsNotNull(result);
            
        }

        [Test]
        public void GnerateQr_Failure_OrderId()
        {
            int orderId = 99;
            var ex=Assert.ThrowsAsync<Exception>(async()=>await _repository.GenerateQr(orderId));
            Assert.That(ex.Message, Does.Contain("OrderId was not found"));
        }



    }
}