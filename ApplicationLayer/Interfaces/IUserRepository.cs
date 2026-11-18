using CoreModels.DTO_S;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IUserRepository
    {

        Task<User>GetUserById(string  id);

        Task CheckEmail(string email);

        Task<UserCreateDto> AddUser(UserCreateDto user);

        Task<LoginResponse>Login(LoginDto login);

        Task UpdatePassword(UpdatePassword dto);

        Task<string> ForgetPassword(string email);

        Task<Guid> VerifyOtp(string email, string otptext);

        Task Resetpassword(Guid token ,string newPassword,string confirmPassword);

        Task UpdateProfile(string userId, UpdateProfileDTO dto);

       
    }
}
