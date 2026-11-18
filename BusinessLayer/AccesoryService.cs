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
    public class AccesoryService
    {
        private readonly IUnitOfWork unitOfWork;
        public AccesoryService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

       

        public async Task<RecommendationResponseDto> GetFullRecommendationsAsync(int productId)
        {
            return await this.unitOfWork.accessRepository.GetFullRecommendations(productId);
        }


        public async Task<string> GetWhyThis(int mainProductId, int recommendedProductId)
        {
            return await this.unitOfWork.accessRepository.GetWhyThis(mainProductId, recommendedProductId);
        }

        public async Task<string> CompareProducts(int product1Id, int product2Id)
        {
            return await this.unitOfWork.accessRepository.CompareProducts(product1Id, product2Id);
        }

        public async Task<ComboResposneDto> GetBestCombo(int productId)
        {
            return await this.unitOfWork.accessRepository.GetBestCombo(productId);
        }

        public async Task<string> OptimizeBudgetProducts(int productId, decimal budget)
        {
            return await this.unitOfWork.accessRepository.OptimizeBudget(productId, budget);
        }

        //public async Task<double> GetPredictionPower(PredictionRequest dto)
        //{
        //    return await this.unitOfWork.accessRepository.GetPredictionPower(dto);
        //}

        public async Task<PowerForecastResponse> GetWhetherForecast(string userId)
        {
            return await this.unitOfWork.accessRepository.GetWhetherForecast(userId);
        }


        public async Task<List<DailyForecast>> GetDailyForecasts(string userId)
        {
            return await this.unitOfWork.accessRepository.GetDailyForecasts(userId);
        }

        public async Task<SolarSizingResponse> CalculateSizing(SolarSizingRequest request)
        {
            return await this.unitOfWork.accessRepository2.CalculateSizing(request);
        }
    }
}
