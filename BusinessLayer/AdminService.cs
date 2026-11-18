using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class AdminService
    {
        private readonly IUnitOfWork unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<AdminDashboardStatsDto> GetStatsData() {
            return await this.unitOfWork.AdminRepository.GetStats();
        }

        public async Task<List<UsersDataDto>> GetAllUsers()
        {
            return await this.unitOfWork.AdminRepository.GetAllUsers();
        }

       
        public async Task<object> GetProductionData()
        {
            return await this.unitOfWork.AdminRepository.GetGlobalProduction();
        }

        public async Task<object> GetProductionTrend()
        {
            return await this.unitOfWork.AdminRepository.ProductionTrend();
        }

        public async Task<object> GetProductionUpdate()
        {
            return await this.unitOfWork.AdminRepository.GetProductionUpdate();
        }

        public async Task<List<object>> SoldProducts()
        {
            return await this.unitOfWork.AdminRepository.SoldProducts();
        }


        public async Task<ProductionUpdateWithAiModel> GetProductionUpdates()
        {
            return await this.unitOfWork.AdminRepository.GetProductionUpdateWithAI();
        }
    }
}
