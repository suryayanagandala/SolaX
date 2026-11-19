using CoreModels.DTO_S;
using DbAccessaLayer.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using SolarXNUnit.SqLite.Base;
using SolarXNUnit.SqLite.SeedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarXNUnit.SqLite.Infrstructure
{
    public class InstallationRepositoryTest:SqliteTestBase
    {
        private IFormFile CreateFakeFile(string fileName)
        {
            var content = "Dummy file content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            return new FormFile(stream, 0, stream.Length, "Data", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };
        }

        private InstallRepository installRepository;
        private readonly FakeSignalRRepository si= new FakeSignalRRepository();
     

        [SetUp]
        public void SetUp()
        {
            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.Setup(x => x.WebRootPath).Returns(Path.GetTempPath());

            installRepository = new InstallRepository(_db,mockEnv.Object,si);
        }

        [Test]
        public async Task GetInstallationStataus_Succes()
        {
            var userId = "ABX123";
            var installation = await _db.Installations.FirstAsync(x => x.InstallationId == "INST-20260201-0001");
            installation.Status = "Assigned";
            await _db.SaveChangesAsync();
            var result = await installRepository.GetInstallationStatus(userId);

            
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
           
        }


        [Test]
        public async Task GetInstallationStatus_Failure_InvalidId()
        {
            var userId = "fakeId";
            var result=await installRepository.GetInstallationStatus(userId);
            Assert.IsEmpty(result);

        }

        [Test]
        public async Task MarkasProgress_Should_Update_Status()
        {
            var installationId = "INST-20260201-0001";

            await installRepository.MarkasProgress(installationId);

            var installation = await _db.Installations.FindAsync(installationId);

            Assert.AreEqual("Completed", installation.Status);
        }

        [Test]
        public void MarkasProgress_InvalidId_Should_Throw_Exception()
        {
            Assert.ThrowsAsync<Exception>(async () =>
                await installRepository.MarkasProgress("INVALID"));
        }

        [Test]
        public async Task UploadInstallationReports_Should_Complete_Installation()
        {
            var dto = new InstallationReportDto
            {
                InstallationId = "INST-20260201-0001",
                Notes = "Test will be success",
                CheckList = "OK",
                Images = new List<IFormFile>
                 {
                   CreateFakeFile("image1.png")

                 }
            };

            var result = await installRepository.UploadInstallationReports(dto);

            var installation = await _db.Installations.FindAsync("INST-20260201-0001");

            Assert.AreEqual("Completed", installation.Status);
            Assert.IsNotNull(result);
        }


        [Test]
        public void UploadInstallationReports_InvalidId_Should_Throw()
        {
            var dto = new InstallationReportDto
            {
                InstallationId = "INVALID",
                Images = new List<IFormFile>
                {
                   CreateFakeFile("image1.png")
                }
            };

            Assert.ThrowsAsync<Exception>(async () =>await installRepository.UploadInstallationReports(dto));
        }


        [Test]
        public void UploadInstallationReports_NoImages_Should_Throw()
        {
            var dto = new InstallationReportDto
            {
                InstallationId = "INST-20260201-0001",
                Notes = "Test will be success",
                CheckList = "OK",
                Images = new List<IFormFile>()
            };

            Assert.ThrowsAsync<Exception>(async () =>
                await installRepository.UploadInstallationReports(dto));
        }
    }
}
