using CoreModels.DTO_S;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IAccessRepository2
    {
        Task<SolarSizingResponse>CalculateSizing(SolarSizingRequest request);
    }
}
