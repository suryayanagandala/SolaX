using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using DbAccessaLayer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using SolarXNUnit.SqLite.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarXNUnit.SqLite.Infrstructure
{
    public class TechnicianRepositoryTest:SqliteTestBase
    {
        private TechnicianRepository technicianRepository;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IConfiguration> _configurationMock;
        [SetUp]
        public void SetUp()
        {
            _emailServiceMock = new Mock<IEmailService>();
            _configurationMock = new Mock<IConfiguration>();
            technicianRepository=new TechnicianRepository(_db,_configurationMock.Object,_emailServiceMock.Object);
        }

        [Test]
        public void AssignedTechnician_InvalidId()
        {

            var dto = new TechnicianCreateDto
            {
                
                Specialization = Specialization.Installer,
                Qualification = Qualification.Diploma,
                Salary = 30000,
                Discipline = Discipline.Mechanical,
                ServicePrice = 1200,
                ExperienceYears = 1

            };
            Assert.ThrowsAsync<DbUpdateException>(async () => await technicianRepository.AssignedTechnician(dto));
        }

        [Test]
        public void AssignedTechnician_Already_Name_Exists_FailureCase()
        {
            string passsword = "Tech@2003";
            var users = new Users
            {
                UserId = "MUMBAI",
                FullName = "Abhi",
                EmailId = "adbhi@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword(passsword),
                Address = "Kondapalli vijaywada",
                PhoneNumber = "9494962341",
                CreatedBy = "Admin",
                CreatedDate = TimeUtility.NowIST(),
                RoleId = (int)RoleIds.Technician
            };

            _db.Users.Add(users);
            _db.SaveChanges();

            var dto = new TechnicianCreateDto
            {
                Specialization = Specialization.Installer,
                Qualification = Qualification.Diploma,
                Discipline = Discipline.Mechanical,
                ExperienceYears = 1,
                Salary = 20000,
                FullName = "Abhi",
                Address="ndihuihfiurf",
                PhoneNumber="8098989891",
                EmailId="abhi@gmail.com",
                ServicePrice=500,

            };
           


             Assert.ThrowsAsync<DataExistsException>(async () => await technicianRepository.AssignedTechnician(dto));
            
        }

        [Test]
        public void AssignedTechnician_Failure_EmailExists()
        {
            string passsword = "Tech@2003";
            var users = new Users
            {
                UserId = "MUMBAI",
                FullName = "Abhi",
                EmailId = "abhi@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword(passsword),
                Address = "Kondapalli vijaywada",
                PhoneNumber = "9494962341",
                CreatedBy = "Admin",
                CreatedDate = TimeUtility.NowIST(),
                RoleId = (int)RoleIds.Technician
            };

            _db.Users.Add(users);
            _db.SaveChanges();

            var dto = new TechnicianCreateDto
            {
                Specialization = Specialization.Installer,
                Qualification = Qualification.Diploma,
                Discipline = Discipline.Mechanical,
                ExperienceYears = 1,
                Salary = 20000,
                FullName = "Abhii",
                Address = "ndihuihfiurf",
                PhoneNumber = "8098989891",
                EmailId = "abhi@gmail.com",
                ServicePrice = 500,

            };


            Assert.ThrowsAsync<DataExistsException>(async () => await technicianRepository.AssignedTechnician(dto));
            

        }

        [Test]
        public void NullDto_PassedException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await technicianRepository.AssignedTechnician(null));

        }

        [Test]
        public async Task AssigendTechnician_Create_Success()
        {

            var dto = new TechnicianCreateDto
            {
                Specialization = Specialization.Installer,
                Qualification = Qualification.Diploma,
                Salary = 30000,
                Discipline = Discipline.Mechanical,
                ServicePrice = 1200,
                ExperienceYears = 1,
                PhoneNumber = "8098989891",
                EmailId = "solar@gmail.com",
                FullName = "solar12",
                Address = "bhcdiuhufoirfjoif",


            };

            await technicianRepository.AssignedTechnician(dto);
            var techniciansCount = await _db.Technicians.CountAsync();
            Assert.That(techniciansCount, Is.EqualTo(3));

        }

        //GetTechnician
        [Test]
        public void GetTechnician_InvalidId_Failure()
        {
            var userId = "fakeId";
            var ex=Assert.ThrowsAsync<NotFoundException>(async () => await technicianRepository.GetTechnician(userId));
            Assert.That(ex.Message, Does.Contain("UserId is not found"));
        }

        [Test]
        public async Task GetTechnician_ValidCase_Success()
        {
            var userId = "ABX123";
            var result = await technicianRepository.GetTechnician(userId);
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.UserId);
        }

        //GetDashboard
        [Test]
        public void GetDahboard_Failure_InvalidId()
        {
            var userId = "fakeId";
           var ex= Assert.ThrowsAsync<NotFoundException>(async()=>await technicianRepository.GetDashboard(null));
            
        }

        [Test]
        public async Task GetDashboard_Success()
        {
            var userId = "ABX123";
            var result = await technicianRepository.GetDashboard(userId);
            Assert.IsNotNull(result);
        }


        //updateTechnician
        [Test]
        public void UpdateTechnicianProfile_InvalidId()
        {
            string userId = "fakeId";
            var dto = new UpdateTechnicianDto
            {

            };
            Assert.ThrowsAsync<NotFoundException>(async () => await technicianRepository.UpdateTechnicianProfile(dto,userId));
        }

        [Test]
        public async Task UpdateTechnicianProfile_Success()
        {
            string userId = "ABX123";
            var dto = new UpdateTechnicianDto
            {
                ServicePrice = 750,
                ExperienceYears = 2,
                Salary = 40000
            };

            await technicianRepository.UpdateTechnicianProfile(dto, userId);

            var data=await _db.Technicians.FirstAsync(x=>x.UserId== userId);
            Assert.That(data.Salary,Is.EqualTo(dto.Salary));
        }


        //Technician Stats
        [Test]
        public void GetTechnicianWorkStatus_Failure()
        {
            string userId = "fakeId";
            Assert.ThrowsAsync<NotFoundException>(async()=>await technicianRepository.GetTechnicianWorkStatus(userId));
        }

        [Test]
        public async Task GetTechnicianWorkStats_Success()
        {
            string userId = "ABX123";
            var result = await technicianRepository.GetTechnicianWorkStatus(userId);
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);
                Assert.That(result.Completed, Is.EqualTo(2));
            });
        }

        [Test]
        public async Task GetTechnicianWorkStats_Maintenance_Failure()
        {
            string userId = "XYZ789";
            var result = await technicianRepository.GetTechnicianWorkStatus(userId);
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);
                Assert.That(result.Completed, Is.EqualTo(1));
            });
        }


        //GetMonthlyCompletedJobs
        [Test]
        public void GetMonthlyCompletedJobs_InavlidId_Failure()
        {
            string userId = "fakeId";
            Assert.ThrowsAsync<NotFoundException>(async () => await technicianRepository.GetMonthlyCompletedJobs(userId));
        }

        [Test]
        public async Task GetMonthlyCompletedJobs_Installer_Success()
        {
            string userId = "ABX123";

            var result = await technicianRepository.GetMonthlyCompletedJobs(userId);
            Assert.IsNotNull(result);

            foreach(var item in result)
            {
                Console.WriteLine(item.Month);
                Console.WriteLine(item.CompletedJobs);
            }

            //Assert.That(result[0].Month, Is.EqualTo("Mar"));
            Assert.That(result[0].CompletedJobs, Is.EqualTo(3));
        }


        [Test]
        public async Task GetMonthlyCompletedJobs_Maintenance_Success()
        {
            string userId = "XYZ789";
            var result = await technicianRepository.GetMonthlyCompletedJobs(userId);
            Assert.IsNotNull(result);

            foreach (var item in result)
            {
                Console.WriteLine(item.Month);
                Console.WriteLine(item.CompletedJobs);
            }

            var data=result[0];
            //Assert.That(data.Month, Is.EqualTo("Mar"));
            Assert.That(data.CompletedJobs, Is.EqualTo(1));
        }


        //GetTechnicianRevenue
        [Test]
        public void GetTechnicianRevenue_InvaliId_Failure()
        {
            string userId = "fakeId";
            Assert.ThrowsAsync<NotFoundException>(async()=>await technicianRepository.GetTechnicianRevenue(userId));
        }

        [Test]
        public async Task GetTechnicianRevenue_Installer_Success()
        {
            var result = await technicianRepository.GetTechnicianRevenue("ABX123");
            var data = result[0];
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);

                Assert.That(data.Revenue, Is.EqualTo(250));
            });
        }


        [Test]
        public async Task GetTechnicianRevenue_Maintennaces_Success()
        {
            var result = await technicianRepository.GetTechnicianRevenue("XYZ789");
            var data = result[0];
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);

                Assert.That(data.Revenue, Is.EqualTo(750));
            });

        }

        //GetTechnicianEfficiency
        [Test]
        public void GetTechnicianEfficiency_Failure()
        {
            Assert.ThrowsAsync<NotFoundException>(async () => await technicianRepository.GetTechnicianEfficiency("fakeId"));
        }

        [Test]
        public async Task GetTechnicianEfficiency_Success_Installer()
        {
            var result = await technicianRepository.GetTechnicianEfficiency("ABX123");
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);
                Assert.That(result.Efficiency, Is.EqualTo(100));
                Assert.That(result.CompletedJobs, Is.EqualTo(1));
                Assert.That(result.TotalJobs, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task GetTechnicianEfficiency_Maintenances_Success()
        {
            var result = await technicianRepository.GetTechnicianEfficiency("XYZ789");
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);
                Assert.That(result.Efficiency, Is.EqualTo(50));
                Assert.That(result.CompletedJobs, Is.EqualTo(1));
                Assert.That(result.TotalJobs, Is.EqualTo(2));
            });
        }
    }
}
