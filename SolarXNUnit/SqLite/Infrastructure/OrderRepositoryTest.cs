using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.DTO_S;
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
    public class OrderRepositoryTest:SqliteTestBase
    {
        private OrderRepository orderRepository;
        private FakeSignalRRepository _fakeSignalRRepository;

        [SetUp]
        public void SetupRepository()
        {
            _fakeSignalRRepository = new FakeSignalRRepository();
            orderRepository = new OrderRepository(_db,_fakeSignalRRepository);

        }
        [Test]
        public async Task PlaceOrder_Should_CreateOrderSuccessfully()
        {
            var dto = new OrderCreateDto
            {
                UserId = "ERCXET",
                DeliveryAddress = "Vijayawada Kondapalli",
                DeliveryName = "Surya",
                DeliveryPhone = "987654321",
                IsBuyNow = true,
                ProductId = 1,
                Quantity = 2,
                CreatedAt = TimeUtility.NowIST()
                
            };

            var result = await orderRepository.PlaceOrder(dto);

            Assert.IsNotNull(result);
            Assert.Greater(result.OrderId, 0);

            var order = _db.Orders.FirstOrDefault(x => x.OrderId == result.OrderId);
            Assert.IsNotNull(order);
            Assert.AreEqual(OrderStatus.PAYMENT_PENDING, order.Status);
        }

        [Test]
        public async Task CancelOrder_Should_SetStatusCancelled()
        {
            
            var order = _db.Orders.First();

            // Act
            await orderRepository.CancelOrder(order.OrderId, order.UserId);

            // Assert
            var cancelledOrder = _db.Orders.First(x => x.OrderId == order.OrderId);
            Assert.AreEqual(OrderStatus.CANCELLED, cancelledOrder.Status);
            Assert.IsFalse(cancelledOrder.IsActive);
        }

        [Test]
        public async Task GetOrderHistory_Should_ReturnOrders()
        {
            var userId = "ERCXET";

            var history = await orderRepository.GetOrderHistory(userId);

            Assert.IsNotNull(history);
           
        }

        [Test]
        public async Task GetOrderHistory_Failure_UserId()
        {
           
            var nonExistingUserId = "NON_EXISTENT";

           
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await orderRepository.GetOrderHistory(nonExistingUserId));

            Assert.That(ex.Message, Is.EqualTo("Order not found"));
        }

        [Test]
        public async Task Track_Delivery()
        {
            var orderId = 1;
            var result=await orderRepository.GetTrackDelivery(orderId);
            Assert.IsNotNull(result);

        }

        [Test]
        public void Track_Delivery_Should_Exception()
        {
            var orderId = 999;
            Assert.ThrowsAsync<Exception>(async()=>await orderRepository.GetTrackDelivery(orderId));
        }

        //order Details
        [Test]
        public void GetOrderDetails_Success()
        {
            var orderId = 2;
            string userId = "ABX123";
            var result = orderRepository.GetOrderDetails(userId, orderId);
            Assert.IsNotNull(result);
        }


        [Test]
        public void GetOrderDetails_Failure()
        {
            var orderId = 1;
            string userId = "fakeid";
            Assert.ThrowsAsync<Exception>(async () => await orderRepository.GetOrderDetails(userId, orderId));
        }


        [Test]
        public void GetUserDetails()
        {
            string userId= "ABX123";
            var result=orderRepository.GetUserDetails(userId);
            Assert.IsNotNull(result);
            //Assert.That(result,Is.GreaterThan(0));  
        }

        [Test]
        public void GetUserDetais_Failure()
        {
            string userId = "fakeId";
            Assert.ThrowsAsync<Exception>(async () => await orderRepository.GetUserDetails(userId));
        }

    }
}
