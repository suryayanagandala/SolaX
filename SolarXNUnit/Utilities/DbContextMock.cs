using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using DbAccessaLayer.Helpers;
using DbAccessaLayer.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SolarXNUnit.HelperMethods;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SolarXNUnit.Utilities
{
    public class DbContextMock
    {
        protected Mock<ApplicationDbContext> _mockDb;
        protected IQueryable<Users> _users;
        protected UserRepository _userRepository;

        protected Mock<IEmailService> _mockEmailService;
        protected Mock<ISignalRRepository> _signalRRepository;
        protected Mock<IDbContextTransaction> _mockTransaction;

        protected List<OtpManager> _otpManager;
        protected List<Users> users;
        protected List<Notifications> _notifications;
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _users = DataInitializer.UsersData();
            _otpManager = DataInitializer.OtpManagers().ToList();
            users = DataInitializer.UsersData().ToList();
            _notifications = new List<Notifications>();
        }

        [SetUp]
        public void Setup()
        {
            // Each test receives fresh data so mutations do not leak into later tests.
            _users = DataInitializer.UsersData();
            _otpManager = DataInitializer.OtpManagers().ToList();
            users = DataInitializer.UsersData().ToList();
            _notifications = new List<Notifications>();
            var roles = DataInitializer.GetRoles();
            foreach (var user in users.Concat(_users))
            {
                user.Role = roles.FirstOrDefault(role => role.RoleId == user.RoleId)
                    ?? new Roles { RoleId = user.RoleId, RoleName = user.RoleId == 3 ? "Admin" : "User" };
            }
         
            // 1️ Initialize basic mocks FIRST

            _mockEmailService = new Mock<IEmailService>();
            _signalRRepository = new Mock<ISignalRRepository>();

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            // 2️ Prepare Users DbSet Mock
            

            var mockSet = new Mock<DbSet<Users>>();
             _mockTransaction = new Mock<IDbContextTransaction>();

            _mockTransaction
                .Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockTransaction
                .Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mockSet.As<IAsyncEnumerable<Users>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Users>(_users.GetEnumerator()));

            mockSet.As<IQueryable<Users>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Users>(_users.Provider));

            mockSet.As<IQueryable<Users>>().Setup(m => m.Expression).Returns(_users.Expression);
            mockSet.As<IQueryable<Users>>().Setup(m => m.ElementType).Returns(_users.ElementType);

            mockSet.As<IQueryable<Users>>().Setup(m => m.GetEnumerator()).Returns(()=>users.GetEnumerator());

      
            // 3️ Create DbContext Options 
         

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .Options;

            _mockDb = new Mock<ApplicationDbContext>(
                options,
                mockHttpContextAccessor.Object
            );

            _mockDb.Setup(x => x.Users).Returns(mockSet.Object);
            _mockDb.Setup(x => x.Set<Users>()).Returns(mockSet.Object);

            var refreshTokenSet = new Mock<DbSet<RefreshTokens>>();
            refreshTokenSet.Setup(x => x.AddAsync(It.IsAny<RefreshTokens>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<RefreshTokens>>((Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<RefreshTokens>)null!));
            _mockDb.Setup(x => x.RefreshTokens).Returns(refreshTokenSet.Object);

            _mockDb.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);

       
            // 4 Mock Database + Transaction
        

            var mockDatabase = new Mock<DatabaseFacade>(_mockDb.Object);

            mockDatabase
                .Setup(db => db.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockTransaction.Object);

            _mockDb.Setup(db => db.Database)
                   .Returns(mockDatabase.Object);

 
            // 5 Mock Notifications DbSet

            var mockNotificationSet = new Mock<DbSet<Notifications>>();
            var queryableNotifications = _notifications.AsQueryable();

            mockNotificationSet.As<IQueryable<Notifications>>().Setup(m => m.Provider).Returns(queryableNotifications.Provider);

            mockNotificationSet.As<IQueryable<Notifications>>().Setup(m => m.Expression).Returns(queryableNotifications.Expression);

            mockNotificationSet.As<IQueryable<Notifications>>().Setup(m => m.ElementType).Returns(queryableNotifications.ElementType);

            mockNotificationSet.As<IQueryable<Notifications>>().Setup(m => m.GetEnumerator()).Returns(() => queryableNotifications.GetEnumerator());

      
            mockNotificationSet.Setup(m => m.Add(It.IsAny<Notifications>())).Callback<Notifications>(n => _notifications.Add(n));

            _mockDb.Setup(x => x.Notifications).Returns(mockNotificationSet.Object);

            


            //  Setup Real IConfiguration for MessageHelper


            var inMemorySettings = new Dictionary<string, string>
            {
                { "User:Register", "User registered successfully" },
                { "User:UserAlreadyExists", "User already exists" },
                 // JWT Settings 
                { "Jwt:Key", "THIS_IS_MY_SUPER_SECRET_KEY_123456" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Jwt:DurationInMinutes", "60" }
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "message.json"))
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            //  Initialize static dictionary
            MessageHelper.Initialize(config);


            //Mock OtpMnager
            var mockSetForOtpManger = new Mock<DbSet<OtpManager>>();
            var querableOtpmanger=_otpManager.AsQueryable();

            mockSetForOtpManger.As<IAsyncEnumerable<OtpManager>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<OtpManager>(querableOtpmanger.GetEnumerator()));

            mockSetForOtpManger.As<IQueryable<OtpManager>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<OtpManager>(querableOtpmanger.Provider));

            mockSetForOtpManger.As<IQueryable<OtpManager>>().Setup(m => m.Expression).Returns(querableOtpmanger.Expression);
            mockSetForOtpManger.As<IQueryable<OtpManager>>().Setup(m => m.ElementType).Returns(querableOtpmanger.ElementType);
            mockSetForOtpManger.As<IQueryable<OtpManager>>().Setup(m => m.GetEnumerator()).Returns(()=>querableOtpmanger.GetEnumerator());

            _mockDb.Setup(x => x.OtpManagers).Returns(mockSetForOtpManger.Object);


            //  Create Repository
            _userRepository = new UserRepository(
                _mockDb.Object,
                _mockEmailService.Object,
                config,
                _signalRRepository.Object
            );
        }
    }
}