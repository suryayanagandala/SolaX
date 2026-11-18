using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using DbAccessaLayer.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SolarProject.Hubs;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SolarProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserService userService;

       

        private readonly ILogger<UserController> logger;
        public UserController(IUnitOfWork unitOfWork,ILogger<UserController> logger)
        {
            this.userService = new UserService(unitOfWork);
            
            this.logger = logger;
        }

        

        [HttpGet("GetUserById")]
        public async Task<OutputResponse> GetUserById([FromQuery] string id)
        {
            try
            {
                var data = await this.userService.GetUserByIdAsync(id);
                if (string.IsNullOrWhiteSpace(id))
                    return new OutputResponse { Status = false, Error = "UserId cannot be empty" };
                logger.LogInformation($"User is checking details of containing id as{id}");
                return new OutputResponse { Status = true, Data = data };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [HttpPost("RegisterUser")]
        public async Task<OutputResponse>Registeration([FromBody]UserCreateDto dto)
        {
            try
            {
                var data = await userService.RegisterUser(dto);
                logger.LogInformation(MessageHelper.Get("User", "UserCreated"));
                return new OutputResponse { Status = true, Data = data };
            }

            catch (Exception ex)
            {
                return  new OutputResponse
                {
                    Status = false,
                    Error = ex.Message
                };
            }
        }

        [HttpPost("LoginRegistration")]
        public async Task<OutputResponse> LoginForm([FromBody] LoginDto login)
        {
            try
            {
                var data= await userService.LoginForm(login);
                logger.LogInformation($"Login Form: {data}");
                return new OutputResponse { Status = true, Data = data };
            }
            catch(Exception ex)
            {
                var errorMessages = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                logger.LogError(errorMessages);
                return new OutputResponse    { Status=false,Error = errorMessages };
            }
        }

        [HttpPost("UpdatePassword")]
        public async Task<OutputResponse> UpdatePassword([FromBody]UpdatePassword dto)
        {
            try
            {
                await userService.UpdatePassoword(dto);
                logger.LogInformation($"Profile is update");
                return new OutputResponse { Status = true, Data = $"{MessageHelper.Get("User", "UpdatePasssword")} {dto.NewPassword}" };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status=false, Error=ex.Message };
            }


        }

        [HttpPost("SentOtpEmail")]
        public async Task<OutputResponse>SentOtpMail([FromBody] EmailDto dto)
        {
            try
            {
                var otptext= await userService.ForgetPassword(dto.Email);
                logger.LogInformation("otp it sent");
                return new OutputResponse { Status = true, Data = otptext };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status=false,Error=ex.Message};
            }
        }

        [HttpPost("VerfiyOtp")]
        public async Task<OutputResponse> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            try
            {
                var response= await userService.VerifyOtp(dto.Email, dto.Otptext);
                logger.LogInformation($"Verify {dto.Email}");
                return new  OutputResponse { Status = true, Data=response };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

        [HttpPost("ResetPassword")]
        public async Task<OutputResponse> ResetPassword([FromBody] ResetPassswordCreateDto dto)
        {
            try
            {
                await userService.Resetpassword(dto.ResetToken, dto.NewPassword, dto.ConfirmPassword);
                logger.LogInformation("Reset the password" + TimeUtility.NowIST());
                return new OutputResponse { Status = true, Data = MessageHelper.Get("User", "ResetPassword") };
            }
            catch (DbUpdateException ex)
            {
                var errorMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : ex.Message;
                logger.LogError(errorMessage);
                return new OutputResponse
                {
                    Status = false,
                    Data = null,
                    Error = errorMessage
                };
            }
        }

        [HttpPatch("UpdateProfile")]
        public async Task<OutputResponse> UpdateProfile([FromQuery] string userId, [FromBody] UpdateProfileDTO dto)
        {
            try
            {
                await userService.UpdateProfile(userId, dto);
                logger.LogInformation($"profile is update of {userId}");
                return new OutputResponse
                {
                    Status = true,
                    Data = MessageHelper.Get("User", "Profile")
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }


        [HttpPost("VerifyRefreshToken")]
        public async Task<OutputResponse> VerifyTokenReponse([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            try
            {
                var tokenResponse=await userService.VerifyTokenResponse(refreshTokenRequest);
                logger.LogInformation($"Refresh token is Validate at {DateTime.Now}");
                return new OutputResponse() { Status = true, Data = tokenResponse };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new OutputResponse { Status = false, Error = ex.Message };
            }
        }

    }
}
