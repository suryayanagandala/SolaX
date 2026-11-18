using CoreModels.DTO_S;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface AiInterfaceRepository
    {
        Task<string> GenerateExplanation(string prompt);

        Task<PredictionWithTime> GetPredictPower();

        Task<List<PredictionRequest>> GetWeatherInputs();

        Task<double> GetPredictedPower(PredictionRequest input);

        Task<List<PredictionRequest>> GetDailyWeatherInputs();
    }
}
