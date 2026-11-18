using CoreModels.DTO_S;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IOtpServiceRepository
    {
        Task SendOtpAsync(string phoneNumber,string userId, string purpose);
        Task<OtpVerificationResult> VerifyOtpAsync(string phoneNumber, string otp, string purpose);
    }
}

