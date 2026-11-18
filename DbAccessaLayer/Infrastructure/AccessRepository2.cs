using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.DTO_S;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure
{
    public class AccessRepository2 : IAccessRepository2
    {
        private readonly ApplicationDbContext _db;
        private readonly AiInterfaceRepository aiInterfaceRepository;
        public AccessRepository2(ApplicationDbContext db, AiInterfaceRepository aiInterfaceRepository)
        {
            _db = db;
            this.aiInterfaceRepository = aiInterfaceRepository;
        }
        public async Task<SolarSizingResponse> CalculateSizing(SolarSizingRequest request)
        {
            try
            {
                double costPerUnit = 6.0;

                double monthlyUnits = request.MonthlyUnits?? (request.MonthlyBill.Value / costPerUnit);

                double sunLightHours = 5.5;

                double requiredKw = monthlyUnits / (sunLightHours * 30);

                double panelCapcity = 0.55;
                int numberOfPanles = (int)Math.Ceiling(requiredKw / panelCapcity);

                double requiredRoof = numberOfPanles * 21.5;

                //Battery
                double dailyUnits = monthlyUnits * 30;
                double backup = dailyUnits * 0.5;
                double minBattery = backup * 0.8;
                double maxBattery = backup * 1.5;

                //Inverter
                double inverterKW = requiredKw * 1.2;

                //Savings
                double monthlySavings = monthlyUnits * costPerUnit;
                double yearlySavings = monthlySavings * 12;


                //  Cost + ROI
                double systemCost = requiredKw * 55000;
                double roi = systemCost / yearlySavings;

                string recommendation = "";

                if (request.RoofSize < requiredRoof)
                {
                    recommendation = "Your roof size is limited. Consider high-efficiency panels.";
                }
                else
                {
                    recommendation = "Your roof is sufficient for installation.";
                }

                

                var dto= new SolarSizingResponse
                {
                    RequiredKW = Math.Round(requiredKw, 2),
                    NumberOfPanels = numberOfPanles,

                    MinBatteryKWh = Math.Round(minBattery, 2),
                    MaxBatteryKWh = Math.Round(maxBattery, 2),

                    InverterKW = Math.Round(inverterKW, 2),

                    EstimatedMonthlySavings = Math.Round(monthlySavings, 2),
                    EstimatedYearlySavings = Math.Round(yearlySavings, 2),

                    EstimatedSystemCost = Math.Round(systemCost, 0),
                    ROIYears = Math.Round(roi, 1),

                    Recommendation = recommendation
                };

                var prompt= BuildPrompt(request, dto);
                var output=await aiInterfaceRepository.GenerateExplanation(prompt);
                dto.Recommendation = output;

                return dto;

            }
            catch (Exception)
            {
                throw;
            }
        }

        private string BuildPrompt(SolarSizingRequest req, SolarSizingResponse res)
        {
            return $@"
            You are a solar energy expert.

            User Details:
            - Location: {req.Location}
            - Monthly Units: {req.MonthlyUnits ?? (req.MonthlyBill / 8)}
            - Roof Size: {req.RoofSize} sq.ft

            System Calculated:
            - System Size: {res.RequiredKW} kW
            - Panels: {res.NumberOfPanels}
            - Battery: {res.MinBatteryKWh}-{res.MaxBatteryKWh} kWh
            - ROI: {res.ROIYears} years

            Generate a short, clear recommendation for the user.
            Keep it simple and helpful.
            ";
        }
    }
}

