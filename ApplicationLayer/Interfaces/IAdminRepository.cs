using CoreModels.DTO_S;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IAdminRepository
    {
        Task<AdminDashboardStatsDto> GetStats();

        Task<List<UsersDataDto>> GetAllUsers();

        Task<object> GetGlobalProduction();

        Task<object> ProductionTrend();

        Task<object> GetProductionUpdate();

        Task<List<object>> SoldProducts();

        Task<ProductionUpdateWithAiModel> GetProductionUpdateWithAI();
    }
}
