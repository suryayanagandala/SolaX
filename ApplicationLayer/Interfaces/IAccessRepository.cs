using CoreModels.DTO_S;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IAccessRepository
    {
        //Task<List<ProductDto>> GetRecommendedAccessoriesBasedOnProductId(int productId);

        Task<RecommendationResponseDto> GetFullRecommendations(int productId);

        Task<string> GetWhyThis(int mainProductId, int recommendedProductId);

        Task<string> CompareProducts(int product1Id, int product2Id);

        Task<ComboResposneDto> GetBestCombo(int productId);

        Task<string> OptimizeBudget(int productId, decimal budget);

        //Task<double> GetPredictionPower(PredictionRequest dto);

        Task<PowerForecastResponse>GetWhetherForecast(string userId);

        Task<List<double>> GetPredictPowerForOneWeek();

        Task<List<DailyForecast>>GetDailyForecasts(string userId);




    }
}
