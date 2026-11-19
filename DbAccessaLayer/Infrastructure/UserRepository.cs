using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using DbAccessaLayer.Helpers;
using iText.Commons.Actions.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DbAccessaLayer.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext _db;

        private readonly IEmailService _emailService;

        private readonly GenearteToken genearteToken;

        private readonly ISignalRRepository signalRRepository;
        public UserRepository(ApplicationDbContext db,IEmailService emailService, IConfiguration configuration
            ,ISignalRRepository signalRRepository)
        {
            _db = db;
            _emailService = emailService;
            this.genearteToken = new GenearteToken(configuration);
            this.signalRRepository = signalRRepository;
        }

       

        public async Task<User> GetUserById(string id)
        {
            try
            {
                var data = await _db.Set<Users>().FirstOrDefaultAsync(x => x.UserId == id);
                if (data == null)
                {
                    throw new UserNotFound(MessageHelper.Get("User","UserId"));
                }
                else
                {
                    var fetchData = new User
                    {
                        UserId = data.UserId,
                        RoleId = data.RoleId,
                        FullName = data.FullName,
                        EmailId = data.EmailId,
                        IsActive = data.IsActive,
                        Address=data.Address,
                        Password=data.Password,
                        PhoneNumber=data.PhoneNumber,
                    };
                    return fetchData;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
                throw;
            }

        }

        public async Task CheckEmail(string email)
        {
            try
            {
                var emailData= await _db.Set<Users>().AnyAsync(x => x.EmailId == email);
                if(emailData) 
                {
                    throw new EmailExists(MessageHelper.Get("User","Email"));
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public  async Task<string> GenerateUserId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random= new Random();
            string id;
            bool exists;
            do
            {
                id=new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());

                 exists = await UserIdExists(id); 
                
            }
            while (exists);

            return id;
        }

        private async Task<bool> UserIdExists(string id)
        {
            return await _db.Users.AnyAsync(x => x.UserId == id);
        }
        public async Task<UserCreateDto> AddUser(UserCreateDto user)
        {
            using var transactions= await _db.Database.BeginTransactionAsync();
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                await CheckEmail(user.EmailId);

                

                if (string.IsNullOrWhiteSpace(user.EmailId))
                    throw new ValidationException("EmailId is required");

                if (string.IsNullOrWhiteSpace(user.Password))
                    throw new ValidationException("Password is required");

                if (string.IsNullOrWhiteSpace(user.FullName))
                    throw new ValidationException("FullName is required");

                if (string.IsNullOrWhiteSpace(user.Address))
                    throw new ValidationException("Address is required");

                if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                    throw new ValidationException("PhoneNumber is required");

                var exists = await _db.Users.AnyAsync(x => x.UserId == user.UserId);

                if (exists)
                    throw new DublicateUserException(MessageHelper.Get("User","UserAlreadyExists"));
                var userData = new Users
                {
                    UserId=await GenerateUserId(),
                    EmailId=user.EmailId.Trim().ToLower(),
                    PhoneNumber=user.PhoneNumber.Trim().ToLower(),
                    RoleId=3,
                    Password=BCrypt.Net.BCrypt.HashPassword(user.Password),
                    FullName=user.FullName.Trim().ToLower(),
                    Address=user.Address.Trim().ToLower(),
                    CreatedBy="Admin"
                };
                _db.Users.Add(userData);

                var notification = new Notifications
                {
                    UserId = userData.UserId,
                    Type = NotificationType.Regsitration,
                    Title = "Registration",
                    IsActive = false,
                    Message = MessageHelper.Get("Messages", "Register")
                };
                await _db.SaveChangesAsync();

                await _db.Notifications.AddAsync(notification);

                var signal = new NotificationDto
                {
                    UserId = notification.UserId,
                    NotificationId = notification.NotificationId,
                    NotificationType = notification.Type.ToString() ?? "UnKnown",
                    Title = notification.Title,
                    Message = notification.Message,
                    IsRead = false,
                    CreatedAt = notification.CreatedAt,
                };

                await this.signalRRepository.SendAsyncNotification(userData.UserId, signal);
                
                await transactions.CommitAsync();
                return user;

            }
            catch (Exception)
            {
                await transactions.RollbackAsync();
                throw;
            }
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        public async Task<LoginResponse>Login(LoginDto login)
        {
            using var transactions = await _db.Database.BeginTransactionAsync();
            try
            {
                var userData = await _db.Users.Include(x=>x.Role).FirstOrDefaultAsync(x => x.EmailId == login.Email);
                if (userData == null)
                {
                    throw new UserNotFound(MessageHelper.Get("User","UserNotFound"));
                }

                bool verifyPassword = BCrypt.Net.BCrypt.Verify(login.Password,userData.Password);
                if (!verifyPassword)
                {
                    throw new UserNotFound(MessageHelper.Get("User","Password"));
                }

                var token = this.genearteToken.GenearteJwtToken(userData, userData.Role.RoleName);

                var refreshToken = GenerateRefreshToken();

                var refreshTokenEntity = new RefreshTokens
                {
                    UserId = userData.UserId,
                    RefreshToken = refreshToken,
                    ExpiryDate = TimeUtility.NowIST().AddDays(7),
                    CreatedAt=TimeUtility.NowIST(),
                };

                await _db.RefreshTokens.AddAsync(refreshTokenEntity);
                await _db.SaveChangesAsync();

                var notification = new Notifications
                {
                    UserId = userData.UserId,
                    Message = "You succesfully login",
                    Type = NotificationType.System,
                    CreatedAt = TimeUtility.NowIST(),

                };
                await _db.Notifications.AddAsync(notification);
                await _db.SaveChangesAsync();
                var dto = new NotificationDto
                {
                    UserId = notification.UserId,
                    NotificationId = notification.NotificationId,
                    NotificationType = notification.Type.ToString()!,
                    Title = notification.Title,
                    Message = notification.Message,
                    IsRead = false,
                    CreatedAt = notification.CreatedAt,

                };

                await signalRRepository.SendAsyncNotification(userData.UserId, dto);
                await transactions.CommitAsync();
                return new LoginResponse
                {
                    TokenResponse=token,
                    Email = userData.EmailId,
                    UserId = userData.UserId,
                    FullName = userData.FullName,
                    RoleId = userData.RoleId,
                    RoleName=userData.Role.RoleName,
                    RefreshToken=refreshToken,
                };
                
            }
            catch (Exception)
            {
                await transactions.RollbackAsync();
                throw;
            }
      }

        public async Task UpdatePassword(UpdatePassword dto)
        {
            var validCredentails = await _db.Users.FirstOrDefaultAsync(x => x.EmailId == dto.Email);
            if(validCredentails == null)
            {
                throw new UserNotFound(MessageHelper.Get("User","Email"));
            }

            bool verifyOldpassword = BCrypt.Net.BCrypt.Verify(dto.OldPassword, validCredentails.Password);
            if (!verifyOldpassword)
            {
                throw new Exception(MessageHelper.Get("User", "OldPassword"));
            }

            if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, validCredentails.Password))
            {
                throw new Exception(MessageHelper.Get("User", "NewPasssword"));
            }

            string newPassword=BCrypt.Net.BCrypt.HashPassword(dto.NewPassword); 

            validCredentails.Password= newPassword;
            validCredentails.UpdatedAt = TimeUtility.NowIST();
            validCredentails.UpdatedBy = validCredentails.FullName;

            await _db.SaveChangesAsync();
        }

        //for otp

        private static string GenerateNumber()
        {
            Random random = new Random();
            var randomNumber=random.Next(0,1000000).ToString("D6");
            return randomNumber;
        }

        public async Task UpdateOtpManager(string email,string otptext,string otptype)
        {
            try
            {
                var checkEmail=await _db.Users.Where(x=>x.EmailId == email).ToListAsync();
                if (!checkEmail.Any())
                {
                    throw new Exception(MessageHelper.Get("User", "Email"));
                }
                else
                {
                   
                    var otpContext = new OtpManager
                    {
                        Email = email,
                        Otptext = otptext,
                        Otptype = otptype,
                        CreatedDate=TimeUtility.NowIST(),
                        Expiration = DateTimeOffset.UtcNow.AddMinutes(5),
                        IsUsed = false
                    };
                    await _db.OtpManagers.AddAsync(otpContext);
                    await _db.SaveChangesAsync();
                }
            }
            catch(Exception )
            {
                throw;
            }
        }

        public async Task<Guid> VerifyOtp(string email,string otptext)
        {
            try
            {
                email = email.Trim().ToLower();
                otptext = otptext.Trim();

                var data = await _db.OtpManagers.Where(x => x.Email.ToLower() == email && x.Otptext == otptext && x.IsUsed == false).OrderByDescending(x => x.Id).FirstOrDefaultAsync();

                if (data == null || data.Expiration < DateTimeOffset.UtcNow)
                    throw new Exception("Invalid or expired OTP");
                data.IsUsed = true;
                data.token = Guid.NewGuid();
                await _db.SaveChangesAsync();

                return data.token;
               

            }
            catch (Exception)
            {
                throw;
            }

        }

        private async Task SendOtpMail(string useremail, string opttext, string username)
        {
            var mailRequest = new MailRequest();
            mailRequest.Email = useremail;
            mailRequest.Subject = "SolarX - Password Reset OTP";
            mailRequest.EmailBody = GenerateEmailBody(username, opttext);
            await _emailService.SendEmail(mailRequest);

        }

        private string GenerateEmailBody(string name, string otptext)
        {
            string emailBody = string.Empty;
            emailBody = "<div style='width:100%; background-color:#342913; color:white;'>";
            emailBody += "<h1>Hi" + name + ", Wecome to SolarX</h1>";
            emailBody += "<h2>Please enter OTP text and update the password </h2>";
            emailBody += "<h2>OTP Text is:" + otptext + "</h2>";
            emailBody += "</div>";
            return emailBody;
        }

        //private async Task SendUpdatePassword(string email,string name)
        //{
        //    var mailRequest = new MailRequest
        //    {
        //        Email = email,
        //        Subject = "Password Updatd Successfully",
        //        EmailBody = $@"<div style='width:100%;padding:20px;'>
        //        <h2>Hi,{name}</h2>
        //        <p>Password Updatd Successfully</p> <br/>
        //        <b>SolarX Team</b>"
              

               
        //    };
        //    await _emailService.SendEmail(mailRequest);
        //}

        private async Task SendRegisteration(string email, string name)
        {
            var mailRequest = new MailRequest
            {
                Email = email,
                Subject = "Registration successfully!",
                EmailBody = $@"<div style='width:100%;padding:20px;'>
                <h2>Hi,{name}</h2>
                <p>Thanks for registration and Welcome to SolarX</p> <br/>
                <b>SolarX Team</b>"



            };
            await _emailService.SendEmail(mailRequest);
        }

        public async Task<string> ForgetPassword(string email)
        {
            try
            {
                var userData=await _db.Users.Where(x=>x.EmailId==email).FirstOrDefaultAsync();
                if (userData == null)
                {
                    throw new UserNotFound(MessageHelper.Get("User", "Email"));
                }
                else
                {
                    string otpNumber = GenerateNumber();
                    await UpdateOtpManager(email,otpNumber,"Forgot Password");
                    //await SendOtpMail(userData.EmailId, otpNumber,userData.FullName);
                    return otpNumber;
                }


            }
            catch (Exception)
            {
                throw;
            }
        }

       private async Task SendNotifications(string userId,string message)
        {

            var sendData = new Notifications
            {
                UserId = userId,
                Message = message,
                Type = CoreModels.Enums.NotificationType.System,
                IsRead = false,
                IsActive = true,
            };

            _db.Notifications.Add(sendData);
            await _db.SaveChangesAsync();
        }


        public async Task Resetpassword(Guid token, string newPassword,string confirmPassword)
        {
            var otpData = await _db.OtpManagers.FirstOrDefaultAsync(x => x.token == token);

            if (otpData == null)
                throw new Exception("Invalid or expired reset request");

            var userData = await _db.Users.Where(x => x.EmailId == otpData.Email).FirstOrDefaultAsync();

            if (userData == null)
                throw new UserNotFound($"User not found with email ");

            //if (BCrypt.Net.BCrypt.Verify(newPassword, userData.Password))
            //    throw new Exception("New password must be different from old password");

            if (newPassword != confirmPassword)
            {
                throw new Exception($"Confirm and new password must be same");
            }

            userData.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            userData.UpdatedAt = TimeUtility.NowIST();

            //checkOtp.IsUsed = true;
            await _db.SaveChangesAsync();

            await SendNotifications(userData.UserId, "Reset password successfully updated!");
            //await SendUpdatePassword(userData.EmailId, userData.FullName);
        }


        public async Task UpdateProfile(string userId, UpdateProfileDTO dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                throw new UserNotFound("User not found");

            user.FullName = dto.FullName.Trim();
            user.PhoneNumber = dto.PhoneNumber.Trim();
            user.Address = dto.Address.Trim();
            user.UpdatedAt = TimeUtility.NowIST();
            user.EmailId = dto.Email!.Trim().ToLower();

            await _db.SaveChangesAsync();
        }


    }
}
