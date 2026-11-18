using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IGenerateToken
    {
        string GenerateJwtToken(Users user, string roleName);
    }
}
