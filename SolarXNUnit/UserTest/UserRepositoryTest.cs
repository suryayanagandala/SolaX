using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using CoreModels.Entities;
using DbAccessaLayer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using SolarXNUnit.HelperMethods;
using SolarXNUnit.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static QRCoder.PayloadGenerator;
using static System.Net.WebRequestMethods;

namespace SolarXNUnit.UserTest
{
    public class UserRepositoryTest : DbContextMock
    {
        //check by userId
        [Test]
        public async Task GetUserById()
        {
            var user = _users.First();
            var userIdExist = await _userRepository.GetUserById(user.UserId);
            Assert.IsNotNull(userIdExist);
            Assert.That(userIdExist.UserId, Is.EqualTo(user.UserId));
        }
        [Test]
        public async Task GetUserById_ShouldReturnUser()
        {
            string userId = "ABX123";
            var user = await _userRepository.GetUserById(userId);
            Assert.IsNotNull(user);
            Assert.That(user.UserId, Is.EqualTo(userId));
        }


        [Test]
        public Task GetByUserId_FailureTest()
        {
            var ex = Assert.ThrowsAsync<UserNotFound>(() => _userRepository.GetUserById("mockdb"));

            Assert.That(ex.Message, Is.Not.Empty);

            return Task.CompletedTask;
        }


        //check email
        [Test]
        public async Task CheckEmailSuccess()
        {
            await _userRepository.CheckEmail("anand@gmail.com");
        }

        [Test]
        public void CheckEmailFailure()
        {
            string existsEmail = _users.First().EmailId;

            var result = Assert.ThrowsAsync<EmailExists>(async () =>
                await _userRepository.CheckEmail(existsEmail));

            Assert.IsNotNull(result);
        }



        [Test]
        public void AddUser_ShouldThrow_NullException()
        {
            var result = Assert.ThrowsAsync<ArgumentNullException>(async () => await _userRepository.AddUser(null!));
            Assert.That(result.ParamName, Is.EqualTo("user"));
            _mockDb.Verify(x => x.SaveChanges(), Times.Never);

        }


        [Test]
        public async Task AddUser_ShouldCommit_WhenSuccessful()
        {
            var dto = new UserCreateDto
            {
                EmailId = "new@test.com",
                Password = "123456",
                FullName = "Test User",
                Address = "Chennai",
                PhoneNumber = "9999999999"
            };



            var result = await _userRepository.AddUser(dto);

            Assert.IsNotNull(result);

            // verify commit
            _mockDb.Object.Database.BeginTransactionAsync().Result.CommitAsync();
           

            _signalRRepository.Verify(x =>
                x.SendAsyncNotification(It.IsAny<string>(), It.IsAny<NotificationDto>()),
                Times.Once);
        }

        [Test]
        public void AddUser_ShouldThrowArgumentNullException_WhenUserIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _userRepository.AddUser(null));

        }

        [Test]
        public void AddUser_ShouldThrowValidationException_WhenEmailIsEmpty()
        {


            var user = new UserCreateDto
            {
                EmailId = "",
                Password = "123",
                FullName = "Test",
                Address = "Chennai",
                PhoneNumber = "9999999999"
            };

            Assert.ThrowsAsync<ValidationException>(async () =>
                await _userRepository.AddUser(user));
        }

        [Test]

        public void AddUser_ShouldThrowValidationException_WhenPasswordIsEmpty()
        {


            var user = new UserCreateDto
            {
                EmailId = "test@gmail.com",
                Password = "",
                FullName = "Test",
                Address = "Chennai",
                PhoneNumber = "9999999999"
            };

            Assert.ThrowsAsync<ValidationException>(async () => await _userRepository.AddUser(user));
        }

        [Test]
        public void AddUser_ShouldThrowValidationException_WhenFullNameIsEmpty()
        {
            var user = new UserCreateDto
            {
                EmailId = "test@gmail.com",
                Password = "Surys12",
                FullName = "",
                Address = "Chennai",
                PhoneNumber = "9999999999"
            };
            Assert.ThrowsAsync<ValidationException>(async () => await _userRepository.AddUser(user));
        }

        [Test]
        public void AddUser_ShouldThrowDuplicationException_UserIdExists()
        {
            // Arrange
            var user = new UserCreateDto
            {
                UserId = _users.First().UserId,
                EmailId = "test@gmail.com",
                Password = "123",
                FullName = "Test",
                Address = "Chennai",
                PhoneNumber = "9999999999"
            };

            // Act & Assert
            Assert.ThrowsAsync<DublicateUserException>(async () =>
                await _userRepository.AddUser(user));
        }


        [Test]

        public void AddUser_ShouldRollback_WhenExceptionOccurs()
        {
            // Arrange
            _mockDb
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB Failure")); // Force failure

            var user = new UserCreateDto
            {
                UserId = "NEW100",
                EmailId = "test@gmail.com",
                Password = "123",
                FullName = "Test",
                Address = "Chennai",
                PhoneNumber = "9999999999"
            };

            // Act
            Assert.ThrowsAsync<Exception>(async () =>
                await _userRepository.AddUser(user));

            // Assert rollback called
            //_mockDb.Verify(x => x.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockTransaction.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public void AddUser_ShoulThrow_UserAlreadyExists()
        {
            _mockDb.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new DbUpdateException());

            var user = new UserCreateDto
            {
                UserId = "YYYYCC",
                EmailId = "anand@gmail.com",
                FullName = "Anand",
                Password = "Anand@123",
                PhoneNumber = "9999999999123",
                Address = "Test Address"
            };

            Assert.ThrowsAsync<DbUpdateException>(async () => await _userRepository.AddUser(user));
        }







        //login
        [Test]
        public void Login_ShouldThrowUserNotFound_WhenEmailDoesNotExist()
        {
            var login = new LoginDto
            {
                Email = "hdhhdhjddlkdld",
                Password = "Surya@2003"
            };
            Assert.ThrowsAsync<UserNotFound>(async () => await _userRepository.Login(login));
        }

        [Test]
        public void Login_ShouldThrowUserNotFound_WhenPasswordIsWrong()
        {
            // Arrange

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correct123");

            var user = new Users
            {
                UserId = "U100",
                EmailId = "test@gmail.com",
                Password = hashedPassword,
                FullName = "Test",
                RoleId = 1,
                Address = "dhsjhfehiei",
                PhoneNumber = "7890987668",
                CreatedBy = "Admin"
            };

            _users = new List<Users> { user }.AsQueryable(); // Override mock data

            var loginDto = new LoginDto
            {
                Email = "test@gmail.com",
                Password = "wrongpassword"
            };

            // Act & Assert
            Assert.ThrowsAsync<UserNotFound>(async () =>
                await _userRepository.Login(loginDto));
        }







        [Test]
        public async Task Login_Success()
        {
            // Arrange
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correct123");

            var user = new Users
            {
                UserId = "U100GF",
                EmailId = "test@gmail.com",
                Password = hashedPassword,
                FullName = "Test User",
                RoleId = 1,
                Address = "123 Main St",
                PhoneNumber = "7890987668",
                CreatedBy = "Admin",
                Role = new Roles
                {
                    RoleId = 1,
                    RoleName = "Admin"
                }
            };

            // Update the IQueryable for this test
            _users = new List<Users> { user }.AsQueryable();

            // Recreate DbSet mock with the new data
            var mockSet = new Mock<DbSet<Users>>();
            mockSet.As<IAsyncEnumerable<Users>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Users>(_users.GetEnumerator()));

            mockSet.As<IQueryable<Users>>()
                .Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Users>(_users.Provider));
            mockSet.As<IQueryable<Users>>().Setup(m => m.Expression).Returns(_users.Expression);
            mockSet.As<IQueryable<Users>>().Setup(m => m.ElementType).Returns(_users.ElementType);
            mockSet.As<IQueryable<Users>>().Setup(m => m.GetEnumerator()).Returns(_users.GetEnumerator());

            // Replace Users DbSet in _mockDb
            _mockDb.Setup(x => x.Users).Returns(mockSet.Object);
            _mockDb.Setup(x => x.Set<Users>()).Returns(mockSet.Object);

            var loginDto = new LoginDto
            {
                Email = "test@gmail.com",
                Password = "correct123"
            };

            // Act
            var result = await _userRepository.Login(loginDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.TokenResponse);
            Assert.AreEqual("test@gmail.com", result.Email);
            Assert.AreEqual("Admin", result.RoleName);
        }


        [Test]
        public void UpdatePassword_ShouldThrowUserNotFound_WhenEmailDoesNotExist()
        {
            var dto = new UpdatePassword
            {
                Email = "notfound@example.com",
                OldPassword = "old123",
                NewPassword = "new123"
            };

            Assert.ThrowsAsync<UserNotFound>(async () => await _userRepository.UpdatePassword(dto));
            _mockDb.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public void UpdatePassword_ShouldThrow_WhenOldPasswordIncorrect()
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctOld");

            var user = new Users
            {
                EmailId = "test@example.com",
                Password = hashedPassword,
                FullName = "Test User",
                RoleId = 1,
                Address = "123 Main St",
                PhoneNumber = "7890987668",
                CreatedBy = "Admin",
                UserId = "GDJIER",
                Role = new Roles
                {
                    RoleId = 1,
                    RoleName = "Admin"
                }
            };
            _users = new List<Users> { user }.AsQueryable();

            var mockSet = new Mock<DbSet<Users>>();
            mockSet.As<IAsyncEnumerable<Users>>()
                   .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                   .Returns(new TestAsyncEnumerator<Users>(_users.GetEnumerator()));
            mockSet.As<IQueryable<Users>>().Setup(m => m.Provider)
                   .Returns(new TestAsyncQueryProvider<Users>(_users.Provider));
            mockSet.As<IQueryable<Users>>().Setup(m => m.Expression).Returns(_users.Expression);
            mockSet.As<IQueryable<Users>>().Setup(m => m.ElementType).Returns(_users.ElementType);
            mockSet.As<IQueryable<Users>>().Setup(m => m.GetEnumerator()).Returns(_users.GetEnumerator());

            _mockDb.Setup(x => x.Users).Returns(mockSet.Object);

            var dto = new UpdatePassword
            {
                Email = "test@example.com",
                OldPassword = "wrongOld",
                NewPassword = "new123"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _userRepository.UpdatePassword(dto));
            Assert.That(ex.Message, Is.EqualTo("Messages not found:User.OldPassword"));
        }





        [Test]
        public async Task UpdatePassword_ShouldUpdatePassword_WhenValid()
        {
            // Arrange
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("old123");
            var user = new Users
            {
                EmailId = "test@example.com",
                Password = hashedPassword,
                FullName = "Test User",
                RoleId = 1,
                Address = "123 Main St",
                PhoneNumber = "7890987668",
                CreatedBy = "Admin",
                UserId = "GDJIER",
                Role = new Roles
                {
                    RoleId = 1,
                    RoleName = "Admin"
                }
            };
            _users = new List<Users> { user }.AsQueryable();

            var mockSet = new Mock<DbSet<Users>>();
            mockSet.As<IAsyncEnumerable<Users>>()
                   .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                   .Returns(new TestAsyncEnumerator<Users>(_users.GetEnumerator()));
            mockSet.As<IQueryable<Users>>().Setup(m => m.Provider)
                   .Returns(new TestAsyncQueryProvider<Users>(_users.Provider));
            mockSet.As<IQueryable<Users>>().Setup(m => m.Expression).Returns(_users.Expression);
            mockSet.As<IQueryable<Users>>().Setup(m => m.ElementType).Returns(_users.ElementType);
            mockSet.As<IQueryable<Users>>().Setup(m => m.GetEnumerator()).Returns(_users.GetEnumerator());

            _mockDb.Setup(x => x.Users).Returns(mockSet.Object);
            _mockDb.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var dto = new UpdatePassword
            {
                Email = "test@example.com",
                OldPassword = "old123",
                NewPassword = "new123"
            };

            // Act
            await _userRepository.UpdatePassword(dto);

            // Assert
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify("new123", user.Password));
        }

        //ResetPassword
        [Test]
        public void ResetPassword_Token_Failure()
        {
            var token = Guid.NewGuid();
            Assert.ThrowsAsync<Exception>(async () => await _userRepository.Resetpassword(token, "oldpassword", "newPassword"));
        }


        [Test]
        public async Task ResetPassword_Success()
        {
            
            var token = Guid.Parse("EA007833-178D-4C11-8A78-C4BA6D897F12");

            var user = _users.First(x => x.EmailId == "surya@gmail.com");

            
            await _userRepository.Resetpassword(token, "NewPass123", "NewPass123");

            
            _mockDb.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce());

            Assert.That(
                BCrypt.Net.BCrypt.Verify("NewPass123", user.Password),
                Is.True
            );

        }

        [Test]
        public void ResetPassword_faliure_PasswordMismatch()
        {
            var token = Guid.Parse("EA007833-178D-4C11-8A78-C4BA6D897F12");

            Assert.ThrowsAsync<Exception>(async () => await _userRepository.Resetpassword(token,"newPassword","confirmpassword"));
        }

        //Update Profile
        [Test]
        public void UpdateProfile_UserId_Faliure()
        {
            var userId = "faleId";
            var dto = new UpdateProfileDTO
            {
                Email = "fake@gmail.com",
                Address = "bde",
                FullName = "jejfie",
                PhoneNumber = "7584994545",

            };
            Assert.ThrowsAsync<UserNotFound>(async () => await _userRepository.UpdateProfile(userId, dto));
        }


        [Test]
        public async Task UpdateProfile_Sucess()
        {
            var user = _users.First();
            var dto = new UpdateProfileDTO
            {
                Email = "prasad@gmail.com",
                Address = "Kr puram banagalore",
                FullName = "Prasad",
                PhoneNumber = "9087654881"
            };

             await _userRepository.UpdateProfile(user.UserId, dto);

         
            

            Assert.That(user.EmailId, Is.EqualTo(dto.Email));
            Assert.That(user.Address, Is.EqualTo(dto.Address));
            Assert.That(user.FullName, Is.EqualTo(dto.FullName));
            Assert.That(user.PhoneNumber, Is.EqualTo(dto.PhoneNumber));

            
            _mockDb.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        //forgetPassword
        [Test]
        public void ForgetPassword_Failure_EmailId()
        {
            var email = "fakeEmail";

            Assert.ThrowsAsync<UserNotFound>(async () =>
                await _userRepository.ForgetPassword(email)
            );

            _mockDb.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task ForgetPassword_sucess()
        {
            //check frst one
            var user = _users.First();
            var result = await _userRepository.ForgetPassword(user.EmailId);
            Assert.IsNotNull(result);
            Assert.That(result.Length, Is.GreaterThan(0));

            //second table
            Assert.That(_otpManager.Count, Is.EqualTo(1));

            //compare values
            Assert.That(_otpManager.Count, Is.EqualTo(1));

            var savedOtp = _otpManager.First();
            Assert.That(savedOtp.Email, Is.EqualTo(user.EmailId));
            Assert.That(savedOtp.IsUsed, Is.False);

            _mockDb.Verify(x=>x.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Once());
        }


        //Update otpManager
        [Test]
        public void UpdateOtpManager_failure_invalidData()
        {
            var email = "fakeemail";
            var ex=Assert.ThrowsAsync<Exception>(async () => await _userRepository.UpdateOtpManager(email, "098765", "password"));
            Assert.That(ex.Message, Does.Contain("Invalid email"));
        }

        [Test]
        public async Task UpdateOtpManger_success()
        {
            var otpManger = _otpManager.First();
            await _userRepository.UpdateOtpManager(otpManger.Email, otpManger.Otptext, otpManger.Otptype);
            _mockDb.Verify(x=>x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once()); 
        }

        //verifyOtp
        [Test]
        public async Task VerifyOtp_success()
        {
            var data=_otpManager.First();
            var result = await _userRepository.VerifyOtp(data.Email, data.Otptext);
            Assert.IsNotNull(result);
           
            _mockDb.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public void VerifyOtp_ThrowException_invalidOtp()
        {
            var data = _otpManager.First();

            Assert.ThrowsAsync<Exception>(async () => await _userRepository.VerifyOtp(data.Email, "097689"));
        }

        [Test]
        public void VerifyOtp_Failure_ExpiredOtp()
        {
            
            var otp = _otpManager.First();

            otp.Email = "test@gmail.com";
            otp.Otptext = "123456";
            otp.IsUsed = false;
            otp.Expiration = DateTimeOffset.UtcNow.AddMinutes(-5); 

            
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _userRepository.VerifyOtp("test@gmail.com", "123456")
            );

            Assert.That(ex.Message, Is.EqualTo("Invalid or expired OTP"));

            _mockDb.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

    }
}
