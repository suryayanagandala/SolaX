using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class SolarPerformanceService
    {
        private readonly IUnitOfWork unitOfWork;
        public SolarPerformanceService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SolarPeformance>> GetUsers()
        {
            return await this.unitOfWork.solarPerformaceRepository.GetAllUsers();
        }

        public async Task<List<SolarPerformance>> GetPerofrmance(string userId,int? panelId=null)
        {
            return await this.unitOfWork.solarPerformaceRepository.GetPerformaces(userId,panelId);
        }

        public async Task<List<MonthlyProductionDto>>GetMonthlyUpdates(string userId,int? panelId = null)
        {
            return await this.unitOfWork.solarPerformaceRepository.GetMonthlyProductionUpdate(userId,panelId);
        }

        public async Task<DashboardSummaryDto>GetCustomerDashboard(string userId, int? panelId = null)
        {
            return await this.unitOfWork.solarPerformaceRepository.GetDashboardSummary(userId,panelId); 


        }

        public async Task AddToPanle()
        {
            await this.unitOfWork.solarPerformaceRepository.AddToPanel();
        }

        public async Task<List<object>>GetActivatedSolarPanles(string userId)
        {
            return await this.unitOfWork.solarPerformaceRepository.GetActivatedPanles(userId);
        }

        public async Task<object>GetPanelDetails(int panelId)
        {
            return await this.unitOfWork.solarPerformaceRepository.GetPanelDetails(panelId);
        }
    }
}
