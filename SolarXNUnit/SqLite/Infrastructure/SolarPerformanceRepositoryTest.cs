using CoreModels;
using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using DbAccessaLayer.Infrastructure;
using Microsoft.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using SolarXNUnit.SqLite.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarXNUnit.SqLite.Infrastructure
{
    public class SolarPerformanceRepositoryTest:SqliteTestBase
    {
        private SolarRepository solarRepository;
        [SetUp]
        public void SetUP()
        {
            var options = Options.Create(new AssignmentSettings
            {
                ChargePerUnit= 1
            });

            solarRepository=new SolarRepository(_db, options);
        }

        [Test]
        public async Task GetAllUsersData_Success()
        {
            var data = await solarRepository.GetAllUsers();
            Assert.IsNotNull(data);
            Assert.That(data.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllUsers_Falure()
        {
            var data = await solarRepository.GetAllUsers();
            Assert.IsNotNull(data);
            Assert.That(data.Count(), Is.EqualTo(2));
        }


        [Test]
        public async Task GetDashboardSummary_Should_Return_Correct_Summary()
        {
            _db.Panels.Add(new Panel
            {
                PanelId=5,
                UserId= "ERCXET",
                PanelName="Solar Panles",
                Capacity="600W"
            });

            await _db.SaveChangesAsync();

            _db.SolarPerformances.Add(new SolarPerformance
            {
                UserId = "ERCXET",
                Power = 2000,
                Voltage = (decimal)229.000,
                Efficiency =90,
                PanelId=1,
                Generated_at = DateTime.Now,
                IsActive = true
            });

            await _db.SaveChangesAsync();

         
            var result = await solarRepository.GetDashboardSummary("ERCXET");
            Console.WriteLine(result.GetType().FullName);
            var props = result.GetType().GetProperties();
            foreach (var p in props)
            {
                Console.WriteLine(p.Name);
            }

            Assert.IsNotNull(result);

            dynamic data = result;

         
            Assert.AreEqual(2000, data.TodayTotal);
            Assert.AreEqual(2000, data.MonthlyPower);
            Assert.Greater(data.LifeTimeSavings, 0);
        }

        [Test]
        public async Task GetDashboardSummary_Faliure()
        {
            string userId = "fakeid";
            var result=await solarRepository.GetDashboardSummary(userId);

            string data = result.LifeTimeSavings.ToString();
            Assert.That(data,Is.EqualTo("0"));
        }

        //GetMonthlyUpdates
        [Test]
        public async Task GetMonthlyUpdates_Failure()
        {
            string userId = "ERCXET";
            var result=await solarRepository.GetMonthlyProductionUpdate(userId);
           
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetMonthlyUpdates_Success()
        {
            _db.SolarPerformances.Add(new SolarPerformance
            {
                UserId = "ERCXET",
                Power = 2000,
                Voltage = (decimal)229.000,
                Efficiency = 90,
                PanelId = 1,
                Generated_at = DateTime.Now,
                IsActive = true
            });

            await _db.SaveChangesAsync();

            string userId = "ERCXET";

            var result = await solarRepository.GetMonthlyProductionUpdate(userId);
            Assert.IsNotEmpty(result);


        }

        //GetPerformnaces
        [Test]
        public async Task GetSolarPerformances_Success()
        {
            _db.Panels.Add(new Panel
            {
                PanelId = 5,
                UserId = "ERCXET",
                PanelName = "Solar Panles",
                Capacity = "600W"
            });

            await _db.SaveChangesAsync();

            _db.SolarPerformances.Add(new SolarPerformance
            {
                UserId = "ERCXET",
                Power = 2000,
                Voltage = (decimal)229.000,
                Efficiency = 90,
                PanelId = 5,
                Generated_at = DateTime.Now,
                IsActive = true
            });

            await _db.SaveChangesAsync();

            string userId = "ERCXET";

            var result=await solarRepository.GetPerformaces(userId);

            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(result);
                Assert.AreEqual(result.Count(), 1);
                Assert.IsNotEmpty(result.First().UserId);

            });
        }


        [Test]
        public void GetPerformaces_Failure_Id()
        {
            string userId = "fakeId";
            Assert.ThrowsAsync<NotFoundException>(async () => await solarRepository.GetPerformaces(userId));
        }


        //AddToPanel

        [Test]
        public async Task AddToPanel_Success()
        {
            _db.Deliveries.Add(new Delivery
            {
                DeliveryId = 20,
                OrderId = 2,
                Status = DeliveryStatus.Delivered.ToString(),
                AssignedAt = TimeUtility.NowIST(),
                AssignedBy = "Admin",
                IsActive = true
            });
            _db.Installations.Add(new Installations
            {
                InstallationId = "INST-TEST-PANEL",
                DeliveryId = 20,
                OrderId = 2,
                Status = InstallationStatus.Completed.ToString(),
                CreatedAt = TimeUtility.NowIST(),
                IsPanelCreated = false
            });
            await _db.SaveChangesAsync();
            await solarRepository.AddToPanel();
            //checking the data is inserted
            Assert.That(await _db.Panels.AnyAsync(), Is.True);

            var installation = await _db.Installations.FirstAsync(i => i.InstallationId== "INST-TEST-PANEL");

            Assert.That(installation.IsPanelCreated, Is.True);
        }


        [Test]
        public async Task GetPanles_Success()
        {
            _db.Panels.Add(new Panel
            {
                PanelId = 6,
                UserId = "ERCXET",
                PanelName = "Solar Panles",
                Capacity = "600W",
                IsActive=true,
            });

            await _db.SaveChangesAsync();

            var result = await solarRepository.GetPanles();

            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(result);
                Assert.That(result.Count(), Is.EqualTo(1));
            });

        }

        //ActivatedPanels
        [Test]
        public async Task GetActicatedPanles_Success()
        {
            _db.Panels.Add(new Panel
            {
                PanelId = 6,
                UserId = "ERCXET",
                PanelName = "Solar Panles",
                Capacity = "600W",
                IsActive = true,
            });

            await _db.SaveChangesAsync();

            var result = await solarRepository.GetActivatedPanles("ERCXET");
            Assert.Multiple(() => {
                Assert.IsNotEmpty(result);
                Assert.That(result.Count(), Is.EqualTo(1));   
            });

        }


        //GetPanleDetails
        [Test]
        public async Task GetPanlesDetails_Success()
        {
            var result = await solarRepository.GetPanelDetails(1);
            Assert.Multiple(() => {
                Assert.IsNotEmpty(result.ToString());
            });
        }

        [Test]
        public async Task GetPanlesDeatils_Falure()
        {
            var result = await solarRepository.GetPanelDetails(1);
            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(result.ToString());
            });
        }


        [Test]
        public async Task GetPanels_Success()
        {

        }

    }
}
