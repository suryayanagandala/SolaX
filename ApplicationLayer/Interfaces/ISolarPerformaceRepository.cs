using CoreModels.DTO_S;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface ISolarPerformaceRepository
    {
       Task<IEnumerable<SolarPeformance>> GetAllUsers();

        Task<List<SolarPerformance>> GetPerformaces(string userId,int? panelId=null);

        Task<List<MonthlyProductionDto>> GetMonthlyProductionUpdate(string userId,int? panelId=null);

        Task<DashboardSummaryDto> GetDashboardSummary(string userId,int?panelId=null);

        Task AddToPanel();

        Task<List<Panel>>GetPanles();

        Task<List<object>> GetActivatedPanles(string userId);

        Task<object> GetPanelDetails(int panelId);



    }
}
