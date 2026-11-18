using CoreModels.DTO_S;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshTokenResponse> ValidateRefreshToken(RefreshTokenRequest refreshTokenRequest);

    }
}
