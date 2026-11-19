using ApplicationLayer.Interfaces;
using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using CoreModels.Enums;
using DbAccessaLayer.Infrastructure;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NuGet.Frameworks;
using SolarXNUnit.SqLite.Base;
using SolarXNUnit.SqLite.SeedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SolarXNUnit.SqLite.Infrastructure
{
    public class MaintenanceRepositoryTest:SqliteTestBase
    {
        private IFormFile CreateFakeImage(string fileName)
        {
            var content = "Dummy image";
            var stream= new MemoryStream(Encoding.UTF8.GetBytes(content));

            return new FormFile(stream, 0, stream.Length, "Data", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };
        }

        private MaintenanceRepository maintenanceRepository;
        private FakeSignalRRepository _fakeSignalRRepository;

        [SetUp]
        public void Setup()
        {
            var mock = new Mock<IWebHostEnvironment>();
            mock.Setup(x=>x.WebRootPath).Returns(Path.GetTempPath());
            _fakeSignalRRepository=new FakeSignalRRepository();
            maintenanceRepository=new MaintenanceRepository(_db,mock.Object,_fakeSignalRRepository);
        }

        [Test]
        public void GetMaintenances_Success()
        {
            string userId = "ERCXET";
            var result= maintenanceRepository.GetMaintenanceStatus(userId);
            Assert.IsNotNull(result);
        }

        //Raise a request
        [Test]
        public void RaiseRequest_Failure()
        {
            var dto = new MaintenanceRequestCreateDto
            {
                UserId="fakeId",
                Description="Power is less",
                InstallationId="fakeId"
            };
            Assert.ThrowsAsync<NotFoundException>(async () => await maintenanceRepository.RaiseComplete(dto));

        }


        [Test]
        public void RaiseRequest_DataExists()
        {

            var dto = new MaintenanceRequestCreateDto
            {
                UserId = "XYZ789",
                Description = "Power is less",
                InstallationId = "INST-20260201-0003",
            };

            Assert.ThrowsAsync<DataExistsException>(async()=>await maintenanceRepository.RaiseComplete(dto));
            
        }

        [Test]
        public async Task RaiseRequest_Success()
        {

            var dto = new MaintenanceRequestCreateDto
            {
                UserId = "LMN456",
                Description = "Power is less",
                InstallationId = "INST-20260201-0004",
            };

            await maintenanceRepository.RaiseComplete(dto);

            var updatedInstallation =await _db.Installations.FindAsync("INST-20260201-0004");

            Assert.IsFalse(updatedInstallation.IsActive);

            var notification = await _db.Notifications.Where(x => x.UserId == "LMN456").OrderByDescending(x => x.NotificationId).FirstAsync();

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(notification);
                Assert.That(notification.Type, Is.EqualTo(NotificationType.Maintenance));
                Assert.That(_db.Notifications.Count(), Is.EqualTo(3));

                Assert.IsTrue(_fakeSignalRRepository.WasCalled);
            });


            var request = await _db.MaintenanceRequests.Where(x => x.InstallationId == "INST-20260201-0004").OrderByDescending(x=>x.CreatedAt).FirstAsync();
            Assert.Multiple(() =>
            {

                Assert.IsNotNull(request);
                Assert.AreEqual("Assigned", request.Status);
                Assert.IsNotEmpty(request.RequestId);
            });

            var maintenanceTech = await _db.MaintenanceTechnicians.OrderByDescending(x=>x.MaintenanceId).FirstAsync();
            var technician = await _db.Technicians.Where(x => x.Specialization == Specialization.Maintenance.ToString()).FirstAsync();

            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(maintenanceTech.MaintenanceId.ToString());
                Assert.That(maintenanceTech.TechnicianId, Is.EqualTo(technician.TechnicianId));
                Assert.That(technician.IsActiveDeliveries, Is.EqualTo(1));
            });
        }

        [Test]
         public async Task GetMaintenancesRequests_Success()
        {
            string userId = "XYZ789";
            var result=await maintenanceRepository.maintenanceRequestDtos(userId);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task UploadMaintenanaceReports_Success()
        {
            var dto = new MaintenanceReportDto
            {
                RequestId = "R3934581",
                Notes = "Fixed inverter",
                ReplacedParts = "Wire",
                Images = new List<IFormFile>
                 {
                   CreateFakeImage("test.png")
                }
            };

          
            var result = await maintenanceRepository.CompletedMaintenanceTask(dto);

 
            var updatedRequest = await _db.MaintenanceRequests.FirstAsync(x => x.RequestId == "R3934581");

            Assert.AreEqual("Completed", updatedRequest.Status);
            Assert.IsFalse(updatedRequest.IsActive);

            var updatedInstallation = await _db.Installations.FirstAsync(x => x.InstallationId == "INST-20260201-0001");

            Assert.IsTrue(updatedInstallation.IsActive);


            var updatedTechnician = await _db.Technicians.FirstAsync(x => x.TechnicianId == 2);

            Assert.AreEqual(0, updatedTechnician.IsActiveDeliveries);

            var report = await _db.MaintenanceReports.FirstOrDefaultAsync(x => x.RequestId == "R3934581");

            Assert.IsNotNull(report);

            
          
        }

        [Test]
        public void UploadMaintenanaceReports_NullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await maintenanceRepository.CompletedMaintenanceTask(null));
        }

        [Test]
        public void UploadMaintenaces_NotFoundException()
        {
            var dto = new MaintenanceReportDto
            {
                RequestId = "NONEXISTENT-REQUEST",
                Notes = "Fixed inverter",
                ReplacedParts = "Wire",
                Images = new List<IFormFile>
                 {
                   CreateFakeImage("test.png")
                }
            };

            Assert.ThrowsAsync<NotFoundException>(async () => await maintenanceRepository.CompletedMaintenanceTask(dto));
        }

        [Test]
        public async Task UploadMaintenaces_DataExistsException()
        {
            var completedRequest = await _db.MaintenanceRequests.FirstAsync(x => x.RequestId == "R3934581");
            completedRequest.Status = "Completed";
            await _db.SaveChangesAsync();
            var dto = new MaintenanceReportDto
            {
                RequestId = "R3934581",
                Notes = "Fixed inverter",
                ReplacedParts = "Wire",
                Images = new List<IFormFile>
                 {
                   CreateFakeImage("test.png")
                }
            };
            Assert.ThrowsAsync<DataExistsException>(async () => await maintenanceRepository.CompletedMaintenanceTask(dto));

        }

        [Test]
        public async Task GetCompltedMaintenances()
        {
            string userId = "XYZ789";
            var result=await maintenanceRepository.GetCompletedMaintenances(userId);
            Assert.IsNotNull(result);
        }

        ////AssignedMaintenances
        //[Test]

    }
}
