using CoreModels.DTO_S;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface ITechnicianRepository
    {
        Task AssignedTechnician(TechnicianCreateDto dto);

        Task<TechnicianResponse> GetTechnician(string userId);

        Task<TechnicianDashboardDto> GetDashboard(string technicianId);

        Task UpdateTechnicianProfile(UpdateTechnicianDto dto,string userId);

        Task<TechnicianWorkStatusDto> GetTechnicianWorkStatus(string userId);

        Task<List<MonthlyCompletedJobsDto>> GetMonthlyCompletedJobs(string userId);

        Task<List<TechnicianRevenueDto>> GetTechnicianRevenue(string userId);

        Task<TechnicianEfficiencyDto> GetTechnicianEfficiency(string userId);

    }
}
