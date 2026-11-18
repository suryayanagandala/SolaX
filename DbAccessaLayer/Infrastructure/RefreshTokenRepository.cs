using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using DbAccessaLayer.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _db;

        private readonly GenearteToken genearteToken;
        public RefreshTokenRepository(ApplicationDbContext db,IConfiguration configuration)
        {
            _db=db;
            genearteToken=new GenearteToken(configuration);
        }
        public async Task<RefreshTokenResponse> ValidateRefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            try
            {
                var storedToken=await _db.RefreshTokens.FirstOrDefaultAsync(x=>x.RefreshToken==refreshTokenRequest.RefreshToken);
                if (storedToken == null)
                {
                    throw new NotFoundException(MessageHelper.Get("RefreshToken", "ExpiryToken"));
                }

                if (storedToken.ExpiryDate < TimeUtility.NowIST())
                {
                    throw new ExpiryException(MessageHelper.Get("RefreshToken", "Expired"));
                }

                var existUser=await _db.Users.Include(x=>x.Role).FirstOrDefaultAsync(x=>x.UserId==storedToken.UserId); 
                if(existUser == null)
                {
                    throw new UserNotFound(MessageHelper.Get("RefreshToken","User"));
                }

                var newAccessToken = genearteToken.GenearteJwtToken(existUser, existUser.Role.RoleName);
                return new RefreshTokenResponse
                {
                    AccessToken = newAccessToken,
                };
                
            }
            catch(Exception ex)
            {
                var errorMessages=ex.InnerException!=null?ex.InnerException.Message:ex.Message;
                throw new Exception(errorMessages);
            }
        }
    }
}
