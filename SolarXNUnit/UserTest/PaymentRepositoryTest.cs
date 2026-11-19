using CoreModels.DTO_S;
using CoreModels.Enums;
using Moq;
using SolarXNUnit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarXNUnit.UserTest
{
    public class PaymentRepositoryTest:PaymentDbMockContext
    {
        [Test]
        public async Task PaymentGateway_COD_Should_Process_Completely()
        {
         
            var dto = new PaymentCreateDto
            {
                OrderId = 1,
                UserId = "ABX123",
                PaymentMethod = "COD"
            };

            var initialStock = _products.First().Stock;

          
            var result = await paymentRepository.PaymentGateway(dto);

           
            Assert.That(result.OrderId, Is.EqualTo(1));
            Assert.That(result.PaymentMethod, Is.EqualTo("COD"));
            Assert.That(result.UserId, Is.EqualTo("ERCXET"));

            
            Assert.That(_orders.First().Status, Is.EqualTo(OrderStatus.CONFIRMED));

           
            Assert.That(_products.First().Stock, Is.EqualTo(initialStock - 1));

          
            Assert.That(_technicians.First().IsActiveDeliveries, Is.EqualTo(1));

          
            Assert.That(_invoices.Count, Is.EqualTo(1));

         
            Assert.That(_notifications.Count, Is.EqualTo(2));

         
            _transaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);


            _signalRRepository.Verify(x =>x.SendAsyncNotification( It.IsAny<string>(), It.IsAny<NotificationDto>()),
                Times.Exactly(2));
        }

        [Test]
        public async Task PaymentGateway_Should_Process_Completely()
        {
            // Arrange
            var dto = new PaymentCreateDto
            {
                OrderId = 1,
                UserId = "ABX123",   
                PaymentMethod = "COD"
            };

            // Act
            var result = await paymentRepository.PaymentGateway(dto);

            // Assert
            Assert.IsNotNull(result);

            // verify payment added
            Assert.IsTrue(_payments.Any(p => p.OrderId == 1));

            // verify notification created
            Assert.IsTrue(_notifications.Count > 0);

            // verify delivery created
            Assert.IsTrue(_deliveries.Count > 0);

            // verify invoice created
            Assert.IsTrue(_invoices.Count > 0);

            // verify transaction committed
            _transaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
