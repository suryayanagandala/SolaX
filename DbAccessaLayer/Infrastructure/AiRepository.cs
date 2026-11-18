using ApplicationLayer.Interfaces;
//using Azure;
using CoreModels.DTO_S;
using CoreModels.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace DbAccessaLayer.Infrastructure
{
    public class AiRepository : AiInterfaceRepository
    {
        private readonly HttpClient _http;
        private readonly OllamaSettings _settings;
        private readonly IConfiguration configuration;

        public AiRepository(HttpClient http,IOptions<OllamaSettings> options,IConfiguration configuration)
        {
            _http = http;
            _settings = options.Value;
            this.configuration = configuration;
        }

        public async Task<string> GenerateExplanation(string prompt)
        {
            var requestBody = new
            {
                model = _settings.Model,
                messages = new[]
                {
                new
                {
                    role = "system",
                    content = "You are a helpful AI that explains solar products clearly and concisely."
                },
                new
                {
                    role = "user",
                    content = prompt
                }
            },
                stream = false,
                options = new
                {
                    temperature = 0.7
                }
            };

            var response = await _http.PostAsync(
                $"{_settings.BaseUrl}/api/chat",
                new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json"
                ));

            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Ollama Error: {response.StatusCode} - {responseText}");
            }

            using var doc = JsonDocument.Parse(responseText);

            return doc.RootElement
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "";
        }

        public async Task<List<PredictionRequest>> GetDailyWeatherInputs()
        {
            try
            {
                var apiKey = configuration["Weather:ApiKey"];
                var url = $"https://api.openweathermap.org/data/2.5/forecast?q=Bangalore,IN&appid={apiKey}&units=metric";
                var response=await _http.GetStringAsync(url);
                var weatherData = JsonSerializer.Deserialize<WeatherResponse>(response);
                var list= new List<PredictionRequest>();
                foreach(var weather in weatherData.list)
                {
                    var time = DateTime.Parse(weather.dt_txt);
                    list.Add(new PredictionRequest
                    {
                        Irradiance=CalculateIrradiance(weather.clouds.all),
                        Temperature=weather.main.temp,
                        CloudCover=weather.clouds.all,
                        Humidity=weather.main.humidity,
                        Time=time,
                        Hour=time.Hour,
                    });
                }
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<double> GetPredictedPower(PredictionRequest input)
        {
            var apiKey = configuration["NonLinearPort:ApiKey"];
            try
            {
                var response=await _http.PostAsJsonAsync(apiKey, input);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AiPredictionResponse>();
                    return result!.PredictedPower;
                }
                return 0;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PredictionWithTime> GetPredictPower()
        {
            try
            {
                var weatherInputs = await GetWeatherInputs();

                var predictions = new List<double>();
                var times = new List<DateTime>(); 

                foreach (var item in weatherInputs)
                {
                    var response = await _http.PostAsJsonAsync(
                        "http://127.0.0.1:5000/predict", item);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<PredicitResponse>();
                        predictions.Add(Math.Round(result.PredictPower, 2));
                    }
                    else
                    {
                        predictions.Add(0);
                    }

                    times.Add(item.Time); 
                }

                return new PredictionWithTime
                {
                    Predictions = predictions,
                    ForecastTimes = times
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<PredictionRequest>> GetWeatherInputs()
        {
            try 
            {
                var apiKey = configuration["Weather:ApiKey"];
                var url = $"https://api.openweathermap.org/data/2.5/forecast?q=Bangalore,IN&appid={apiKey}&units=metric";
                var response=await _http.GetStringAsync(url);
                var weatherData = JsonSerializer.Deserialize<WeatherResponse>(response);
                var input = new List<PredictionRequest>();
                foreach(var item in weatherData.list.Take(8))
                {
                    var time = DateTime.Parse(item.dt_txt);
                    double irradiance = CalculateIrradiance(item.clouds.all);
                    input.Add(new PredictionRequest
                    {
                        Irradiance = irradiance,
                        Temperature = item.main.temp,
                        CloudCover = item.clouds.all,   
                        Humidity = item.main.humidity,
                        Time= time,
                        Hour=time.Hour
                    });
                }
                return input;

            }
            catch (Exception)
            {
                throw;
            }
        }

        private double CalculateIrradiance(double cloudCover)
        {
            double maxIrradiance = 1000;
            return maxIrradiance * (1 - (cloudCover / 100));
        }
    }
}
