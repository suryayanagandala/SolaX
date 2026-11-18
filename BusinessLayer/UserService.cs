using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using MimeKit.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class UserService
    {
        private IUnitOfWork unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        public async Task<User> GetUserByIdAsync(string id)
        {
            return await this.unitOfWork.UserRepository.GetUserById(id);
        }

        public async Task<UserCreateDto> RegisterUser(UserCreateDto dto)
        {

            return await unitOfWork.UserRepository.AddUser(dto);    
        }
        public async Task<LoginResponse>LoginForm(LoginDto dto)
        {
            return await this.unitOfWork.UserRepository.Login(dto);
        }

        public async Task UpdatePassoword(UpdatePassword dto)
        {
             await this.unitOfWork.UserRepository.UpdatePassword(dto);
        }

        public async Task<string> ForgetPassword(string email)=>await this.unitOfWork.UserRepository.ForgetPassword(email);

        public async Task Resetpassword(Guid token, string newPassword, string confirmPassword)
        {
            await this.unitOfWork.UserRepository.Resetpassword(token,newPassword,confirmPassword);
        }

        public async Task UpdateProfile(string userId, UpdateProfileDTO dto)
        {
            await this.unitOfWork.UserRepository.UpdateProfile(userId, dto);
        }

        public async Task<Guid> VerifyOtp(string email, string otptext)
        {
           return  await this.unitOfWork.UserRepository.VerifyOtp(email, otptext);
        }

        public async Task<RefreshTokenResponse> VerifyTokenResponse(RefreshTokenRequest refreshTokenRequest)
        {
             return await this.unitOfWork.refreshTokenRepository.ValidateRefreshToken(refreshTokenRequest);
        }
       
    }
}
