using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class TechnicianService
    {
        private readonly IUnitOfWork unitOfWork;

        public TechnicianService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task CreateTechnician(TechnicianCreateDto dto)
        {
            await this.unitOfWork.TechnicianRepository.AssignedTechnician(dto);
        }

        public async Task<TechnicianResponse>GetTechncianResponse(string userId)
        {
            return await this.unitOfWork.TechnicianRepository.GetTechnician(userId);
        
        }

        public async Task UpdateTechnicianProfile(UpdateTechnicianDto dto, string userId)
        {
            await this.unitOfWork.TechnicianRepository.UpdateTechnicianProfile(dto, userId);
        }


        public async Task<TechnicianDashboardDto>GetDashboard(string userId)
        {
            return await this.unitOfWork.TechnicianRepository.GetDashboard(userId);
        }

        public async Task<TechnicianWorkStatusDto> GetTechnicianStats(string userId)
        {
            return await this.unitOfWork.TechnicianRepository.GetTechnicianWorkStatus(userId);
        }

        public async Task<List<MonthlyCompletedJobsDto>> GetMonthlyCompletedTasks(string userId)
        {
            return await this.unitOfWork.TechnicianRepository.GetMonthlyCompletedJobs(userId);
        }

        public async Task<List<TechnicianRevenueDto>>GetTechnicianRevenue(string userId)
        {
            return await this.unitOfWork.TechnicianRepository.GetTechnicianRevenue(userId);
        }

        public async Task<TechnicianEfficiencyDto> GetTechnicianEfficiency(string userId)
        {
            return await this.unitOfWork.TechnicianRepository.GetTechnicianEfficiency(userId);
        }
    }
}
