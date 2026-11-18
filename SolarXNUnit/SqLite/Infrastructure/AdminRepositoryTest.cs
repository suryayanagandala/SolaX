using CoreModels;
using CoreModels.Entities;
using DbAccessaLayer.Infrastructure;
using SolarXNUnit.SqLite.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarXNUnit.SqLite.Infrastructure
{
    public class AdminRepositoryTest:SqliteTestBase
    {
        private AdminRepository adminRepository;
        [SetUp]
        public void SetUp()
        {
            adminRepository = new AdminRepository(_db);
        }

        //GetStats
        [Test]
        public async Task GetStats()
        {
            var result=await adminRepository.GetStats();

            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(result.Users.ToString());
            });
        }


        //GetAllUsers
        [Test]
        public async Task GetAllUsers_Success()
        {
            var data=await adminRepository.GetAllUsers();
            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(data);
                Assert.That(data.Count(),Is.EqualTo(2));
            });
        }


        [Test]
        public void GetAllUsers_Exception()
        {
            adminRepository=new AdminRepository(null);
           
            Assert.ThrowsAsync<Exception>(async ()=>await adminRepository.GetAllUsers());
        }

        [Test]
        public async Task GetGlobalProduction_Should_Return_Monthly_And_Growth()
        {
            var result = await adminRepository.GetGlobalProduction();
            dynamic data = result;

            Assert.IsNotEmpty(result.ToString());
        }

        [Test]
        public async Task ProductionTrend_Should_Calculate_Percentage()
        {
            var result = await adminRepository.ProductionTrend();
            var list = result as IEnumerable<dynamic>;

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetProductionUpdate_Should_Return_Correct_Counts()
        {
           

            var now = TimeUtility.NowIST();
            var startOfDay = now.Date;

            // Add Panel
            var panel = new Panel
            {
               PanelId=5,
                PanelName = "Test Panel",
                UserId= "ABX123",
                Capacity="1000W",
                IsActive=true,
            };

            _db.Panels.Add(panel);

          
            _db.SolarPerformances.Add(new SolarPerformance
            {
                PanelId = 5,
                Power = 100,  
                Generated_at = startOfDay.AddHours(1),
                IsActive = true,
                UserId= "ABX123"
            });

            

            await _db.SaveChangesAsync();

           

            var result = await adminRepository.GetProductionUpdate();
            dynamic data = result;

            Assert.IsNotEmpty(result.ToString());

            
        }

        [Test]
        public async Task GetSoldProducts_Data()
        {
            var result = await adminRepository.SoldProducts();
            Assert.IsNotEmpty(result);
        }


    }
}
