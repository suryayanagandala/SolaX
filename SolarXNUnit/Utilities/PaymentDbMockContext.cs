using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using DbAccessaLayer.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Moq;
using SolarXNUnit.HelperMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarXNUnit.Utilities
{
    public class PaymentDbMockContext
    {
        protected Mock<ApplicationDbContext> _mockDb;
        protected Mock<ISignalRRepository> _signalRRepository;
        protected PaymentRepository paymentRepository;
        protected Mock<IDbContextTransaction> _transaction;
        protected IOptions<AssignmentSettings> _options;

        protected List<Orders> _orders;
        protected List<Products> _products;
        protected List<Technicians> _technicians;
        protected List<Cart> _carts;
        protected List<Payments> _payments;
        protected List<Notifications> _notifications;
        protected List<Invoices> _invoices;
        protected List<Delivery> _deliveries;
        protected List<Users> _users;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _products = DataInitializer.GetProducts();
            _orders = DataInitializer.GetOrders();
            _technicians = DataInitializer.GetTechnicians();
            _carts = DataInitializer.GetCarts();
            _payments = DataInitializer.GetPayments();
            _notifications = new List<Notifications>();
            _invoices = new List<Invoices>();
            _deliveries = new List<Delivery>();
            _users=new List<Users>();
        }

        [SetUp]
        public void Setup()
        {
            // Rebuild the mocked database for every test to prevent cross-test mutations.
            _products = DataInitializer.GetProducts();
            _orders = DataInitializer.GetOrders();
            _technicians = DataInitializer.GetTechnicians();
            _carts = DataInitializer.GetCarts();
            _payments = DataInitializer.GetPayments();
            _notifications = new List<Notifications>();
            _invoices = new List<Invoices>();
            _deliveries = new List<Delivery>();
            _users = DataInitializer.GetUsers();
            foreach (var order in _orders)
            {
                order.User = _users.Single(user => user.UserId == order.UserId);
                foreach (var item in order.OrderItem)
                {
                    item.Order = order;
                    item.Product = _products.Single(product => product.ProductId == item.ProductId);
                }
            }
            _signalRRepository = new Mock<ISignalRRepository>();
            _signalRRepository.Setup(x => x.SendAsyncNotification(It.IsAny<string>(), It.IsAny<NotificationDto>())).Returns(Task.CompletedTask);
            _transaction = new Mock<IDbContextTransaction>();

            _transaction.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            _transaction.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
            var mockHttp = new Mock<IHttpContextAccessor>();
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                _mockDb = new Mock<ApplicationDbContext>(optionsBuilder.Options,mockHttp.Object);

                // Attach DbSets
                AttachDbSet(_orders, x => _mockDb.Setup(db => db.Orders).Returns(x));
                AttachDbSet(_users, x => _mockDb.Setup(db => db.Users).Returns(x));
                AttachDbSet(_products, x => _mockDb.Setup(db => db.Products).Returns(x));
                AttachDbSet(_payments, x => _mockDb.Setup(db => db.Payments).Returns(x));
                AttachDbSet(_technicians, x => _mockDb.Setup(db => db.Technicians).Returns(x));
                AttachDbSet(_carts, x => _mockDb.Setup(db => db.Carts).Returns(x));
                AttachDbSet(_notifications, x => _mockDb.Setup(db => db.Notifications).Returns(x));
                _mockDb.Setup(db => db.Set<Notifications>()).Returns(_mockDb.Object.Notifications);
                Mock.Get(_mockDb.Object.Notifications).Setup(db => db.AddAsync(It.IsAny<Notifications>(), It.IsAny<CancellationToken>()))
                    .Callback<Notifications, CancellationToken>((notification, _) => _notifications.Add(notification))
                    .Returns(new ValueTask<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Notifications>>((Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Notifications>)null!));
                AttachDbSet(_invoices, x => _mockDb.Setup(db => db.Invoices).Returns(x));
                AttachDbSet(_deliveries, x => _mockDb.Setup(db => db.Deliveries).Returns(x));

                _mockDb.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(1);

                var mockDatabase = new Mock<DatabaseFacade>(_mockDb.Object);
                mockDatabase.Setup(db => db.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                            .ReturnsAsync(_transaction.Object);

                _mockDb.Setup(db => db.Database).Returns(mockDatabase.Object);

                _options = Options.Create(new AssignmentSettings
                {
                    AssignedDelivery = 2
                });

            paymentRepository = new PaymentRepository(
                    _mockDb.Object,
                    _options,
                    _signalRRepository.Object
                );
            }



        protected void AttachDbSet<T>(List<T> list, Action<DbSet<T>> attach)
    where T : class
        {
            var queryable = list.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(queryable.Provider));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression)
                .Returns(queryable.Expression);

            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType)
                .Returns(queryable.ElementType);

            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator())
                .Returns(() => queryable.GetEnumerator());

            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));

          
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] ids) =>
                {
                    var id = ids[0];
                    var prop = typeof(T).GetProperties()
                        .FirstOrDefault(p => p.Name.EndsWith("Id"));
                    return list.FirstOrDefault(x =>
                        prop != null && prop.GetValue(x).Equals(id));
                });

            mockSet.Setup(m => m.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
                .Callback<T, CancellationToken>((entity, _) => list.Add(entity))
                .Returns(new ValueTask<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T>>((Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T>)null!));

            attach(mockSet.Object);
        }
    }

    
}
