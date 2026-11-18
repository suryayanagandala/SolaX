using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.DTO_S;
using DbAccessaLayer.Helpers;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using SolarXNUnit.SqLite.SeedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SolarXNUnit.SqLite.Base
{
    public abstract class SqliteTestBase
    {
        protected ApplicationDbContext _db;
        private SqliteConnection _connection;
        protected IHttpContextAccessor HttpContextAccessor;
    
   

        [SetUp]
        public void BaseSetUp()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .Options;

            // Create a fake HttpContext for claims
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim("UserId", "TestUser123"),
        new Claim(ClaimTypes.Role, "Admin")
    }));

            HttpContextAccessor = new HttpContextAccessor
            {
                HttpContext = context
            };

            // Initialize DbContext first
            _db = new ApplicationDbContext(options, HttpContextAccessor);

            // Disable audit logs for tests
            _db.DisableAudit = true;
            _db.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF;");
            _db.Database.EnsureCreated();


            var mockSignalR = new Mock<ISignalRRepository>();

            mockSignalR
                .Setup(x => x.SendAsyncNotification(It.IsAny<string>(), It.IsAny<NotificationDto>()))
                .Returns(Task.CompletedTask);
            // Seed test data
            TestDataSeeder.SeedBasicOrder(_db);
            var inMemorySettings = new Dictionary<string, string>
            {
                ["Order:OrderNotFound"] = "Order not found",
                ["Installation:Installed"] = "Installation already exists"
            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            MessageHelper.Initialize(config);
        }

        [TearDown]
        public void BaseTearDown()
        {
            _db.Dispose();
            _connection.Close();
        }
    }
}
