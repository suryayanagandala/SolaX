using ApplicationLayer.Interfaces;
//using Azure;
using CoreModels;
using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using DbAccessaLayer.Helpers;
//using iText.Signatures.Validation.Lotl;
//using MailKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
//using static Azure.Core.HttpHeader;

namespace DbAccessaLayer.Infrastructure
{
    public class AccessoryRepository : IAccessRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly AiInterfaceRepository _aiInterfaceRepository;
        private readonly ILogger<AccessoryRepository> logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISignalRRepository signalRRepository;
        public AccessoryRepository(ApplicationDbContext db, AiInterfaceRepository aiInterfaceRepository,ILogger<AccessoryRepository> logger,
            IHttpContextAccessor httpContextAccessor, ISignalRRepository signalRRepository)
        {
            _db = db;
            _aiInterfaceRepository = aiInterfaceRepository;
            this.logger = logger;
            _httpContextAccessor = httpContextAccessor;
            this.signalRRepository = signalRRepository;
        }

        public async Task<List<int>> GetAccessoryCategoryIds(int sourceCategoryId)
        {
            try
            {
                var mappedids=await _db.AccessoryCategoryMappings.Where(x=>x.SourceCategoryId==sourceCategoryId).Select(x=>x.AccessoryCategoryId).ToListAsync();
                if(mappedids.Any()) return mappedids;

                var accessoriesParentId = await _db.Categories.Where(c => c.Slug == "accessories").Select(c => c.CategoryId)
                                         .FirstOrDefaultAsync();

                var fallbackIds = await _db.Categories.Where(c => c.ParentCategoryId == accessoriesParentId)
                    .Select(c => c.CategoryId).ToListAsync();

                return fallbackIds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ProductDto>> GetRecommendedAccessoriesBasedOnProductId(int productId)
        {
            try
            {
                var product=await _db.Products.Where(x=>x.ProductId==productId &&x.IsActive).FirstOrDefaultAsync();
                if (product == null)
                {
                    throw new NotFoundException(MessageHelper.Get("Products", "NotFound"));
                }

                var sourceCategoryId=product.CategoryId;

                var getRecommnederCategoryIds=await GetAccessoryCategoryIds(product.CategoryId);

                var accessoriesProducts = await _db.Products.Include(x => x.ProductImages).Where(x => getRecommnederCategoryIds.Contains(x.CategoryId) && x.IsActive && x.ProductId != productId)
                    .OrderByDescending(x => x.AverageRating ?? 0).ThenBy(x => x.Price).Take(6)
                    .Select(x => new ProductDto
                    {
                        ProductId = x.ProductId,
                        CategoryId = x.CategoryId,
                        IsActive = x.IsActive,
                        Brand = x.Brand,
                        Stock=x.Stock,
                        Warranty_Years=x.Warranty_Years,
                        Type = x.Type,
                        Description = x.Description,
                        ProductName=x.ProductName,
                        Price = x.Price,
                        Capacity = x.Capacity,
                        CapacityValue = x.CapacityValue,
                        ProductImages = x.ProductImages.Select(x => new ProductImageResponseDto
                        {
                            ProductId = x.ProductId,
                            ImageId = x.ImageId,
                            ImageUrl = x.ImageUrl,
                        }).ToList()

                    }).ToListAsync();

                return accessoriesProducts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public async Task<List<ProductDto>> GetRecommendedBatteries(double minKWh, double maxKWh)
        //{
        //    try
        //    {
        //        // 1. Get Battery Parent CategoryId
        //        var batteryParentId = await _db.Categories.Where(x => x.Slug.ToLower() == "batteries").Select(x => x.CategoryId).FirstOrDefaultAsync();

        //        // 2. Get Battery Subcategories
        //        var batterySubCategories = await _db.Categories.Where(x => x.ParentCategoryId == batteryParentId).Select(x => x.CategoryId).ToListAsync();

        //        // 3. Get Products
        //        var batteryProducts = await _db.Products.Include(x => x.ProductImages)
        //            .Where(x => batterySubCategories.Contains(x.CategoryId) && x.IsActive).ToListAsync();


        //        var convertedList = batteryProducts
        //            .Select(p => new
        //            {
        //                Product = p,
        //                KWh = ConvertToKWh(p)
        //            })
        //            .Where(x => x.KWh > 0)
        //            .ToList();


        //        // 5. MAIN FILTER 
        //        var filtered = convertedList
        //            .Where(x => x.KWh >= minKWh && x.KWh <= maxKWh)
        //            .OrderByDescending(x => x.Product.AverageRating ?? 0)
        //            .ThenBy(x => x.Product.Price)
        //            .Take(6)
        //            .ToList();

        //        // 6. RELAXED RANGE 
        //        if (!filtered.Any())
        //        {
        //            filtered = convertedList
        //                .Where(x => x.KWh >= minKWh * 0.5 && x.KWh <= maxKWh * 2.0)
        //                .OrderByDescending(x => x.Product.AverageRating ?? 0)
        //                .ThenBy(x => x.Product.Price)
        //                .Take(6)
        //                .ToList();
        //        }

        //        // 7. FINAL FALLBACK 
        //        if (!filtered.Any())
        //        {
        //            filtered = convertedList
        //                .OrderByDescending(x => x.Product.AverageRating ?? 0)
        //                .ThenBy(x => x.Product.Price)
        //                .Take(6)
        //                .ToList();
        //        }


        //        var result = filtered.Select(x => new ProductDto
        //        {
        //            ProductId = x.Product.ProductId,
        //            ProductName = x.Product.ProductName,
        //            CategoryId = x.Product.CategoryId,
        //            IsActive = x.Product.IsActive,
        //            Stock = x.Product.Stock,
        //            Warranty_Years = x.Product.Warranty_Years,
        //            Brand = x.Product.Brand,
        //            Type = x.Product.Type,
        //            Description = x.Product.Description,
        //            Price = x.Product.Price,
        //            Capacity = x.Product.Capacity,
        //            CapacityValue = x.Product.CapacityValue,

        //            ProductImages = x.Product.ProductImages.Select(img => new ProductImageResponseDto
        //            {
        //                ProductId = x.Product.ProductId,
        //                ImageId = img.ImageId,
        //                ImageUrl = img.ImageUrl
        //            }).ToList()

        //        }).ToList();

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error in GetRecommendedBatteries: " + ex.Message);
        //    }
        //}

        public async Task<List<ProductDto>> GetRecommendedBatteries(double requiredKWh)
        {
            var batteryProducts = await _db.Products
                .Include(x => x.ProductImages)
                .Where(x => x.IsActive)
                .ToListAsync();

            var convertedList = batteryProducts
                .Select(p => new
                {
                    Product = p,
                    KWh = ConvertToKWh(p)
                })
                .Where(x => x.KWh > 0)
                .ToList();

            var filtered = convertedList
                .Select(x => new
                {
                    x.Product,
                    x.KWh,
                    Difference = Math.Abs(x.KWh - requiredKWh)
                })
                .OrderBy(x => x.Difference)
                .ThenByDescending(x => x.Product.AverageRating ?? 0)
                .ThenBy(x => x.Product.Price)
                .Take(6)
                .ToList();

            return filtered.Select(x => new ProductDto
            {
                ProductId = x.Product.ProductId,
                ProductName = x.Product.ProductName,
                Price = x.Product.Price,
                Capacity = x.Product.Capacity,
                CapacityValue = x.Product.CapacityValue,
                ProductImages = x.Product.ProductImages.Select(img => new ProductImageResponseDto
                {
                    ImageUrl = img.ImageUrl
                }).ToList()
            }).ToList();
        }

        private double ConvertToKWh(Products product)
        {
            var capacity = (product.Capacity ?? "").ToLower().Trim().Replace(" ", "");

            if (capacity.Contains("ah"))
            {
                // Assume 12V battery
                return (product.CapacityValue * 12) / 1000.0;
            }
            if (capacity.Contains("kwh"))
            {
                return product.CapacityValue;
            }

            return 0;
        }

        public async Task<List<ProductDto>> GetRecommendedInverters(double minWatt, double maxWatt)
        {
            try
            {
                // 1. Get parent inverter category
                var parentInverterCategoryId = await _db.Categories.Where(x => x.Slug == "inverters").Select(x => x.CategoryId)
                                               .FirstOrDefaultAsync();

                var inverterCategories = await _db.Categories.Where(x => x.ParentCategoryId == parentInverterCategoryId)
                                         .Select(x => x.CategoryId).ToListAsync();

                var inverterProducts = await _db.Products.Include(x => x.ProductImages).Where(x => inverterCategories.Contains(x.CategoryId) && x.IsActive)
                                         .ToListAsync();

                
                var filtered = inverterProducts
                    .Select(p => new
                    {
                        Product = p,
                        Watt = ConvertIntoWatt(p)
                    })
                    .Where(x => x.Watt >= minWatt && x.Watt <= maxWatt)
                    .OrderByDescending(x => x.Product.AverageRating ?? 0)
                    .ThenBy(x => x.Product.Price)
                    .Take(6)
                    .ToList();

                
                if (!filtered.Any())
                {
                    filtered = inverterProducts
                        .Select(p => new
                        {
                            Product = p,
                            Watt = ConvertIntoWatt(p)
                        })
                        .Where(x => x.Watt >= minWatt * 0.8 && x.Watt <= maxWatt * 1.5)
                        .OrderByDescending(x => x.Product.AverageRating ?? 0)
                        .ThenBy(x => x.Product.Price)
                        .Take(6)
                        .ToList();
                }

                if (!filtered.Any())
                {
                    filtered = inverterProducts
                        .Select(p => new
                        {
                            Product = p,
                            Watt = ConvertIntoWatt(p)
                        })
                        .OrderBy(p => Math.Abs(p.Watt - minWatt))
                        .Take(3)
                        .ToList();
                }


                var result = filtered.Select(x => new ProductDto
                {
                    ProductId = x.Product.ProductId,
                    ProductName = x.Product.ProductName,
                    CategoryId = x.Product.CategoryId,
                    IsActive = x.Product.IsActive,
                    Stock = x.Product.Stock,
                    Warranty_Years = x.Product.Warranty_Years,
                    Brand = x.Product.Brand,
                    Type = x.Product.Type,
                    Description = x.Product.Description,
                    Price = x.Product.Price,
                    Capacity = x.Product.Capacity,
                    CapacityValue = x.Product.CapacityValue,

                    ProductImages = x.Product.ProductImages.Select(img => new ProductImageResponseDto
                    {
                        ProductId = x.Product.ProductId,
                        ImageId = img.ImageId,
                        ImageUrl = img.ImageUrl
                    }).ToList()
                }).ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private double ConvertIntoWatt(Products products)
        {
            var capacity= products.Capacity?.ToLower() ?? string.Empty;
            if (capacity.Contains("va"))
            {
                return products.CapacityValue * 0.8; //power factor
            }

            else if (capacity.Contains("kw"))
            {
                return products.CapacityValue * 1000;
            }
            else
            {
                return 0;
            }
        }

        public async Task<RecommendationResponseDto> GetFullRecommendations(int productId)
        {
            try
            {
                var recomendations = new RecommendationResponseDto();

                var product=await _db.Products.Include(x=>x.Category).Where(x=>x.ProductId==productId&&x.IsActive).FirstOrDefaultAsync();

                if (product==null)
                {
                    throw new NotFoundException(MessageHelper.Get("Products", "NotFound"));
                }

                //get accessories
                recomendations.Accessories=await GetRecommendedAccessoriesBasedOnProductId(product.ProductId);

                var capacity=product.Capacity?.ToLower() ?? string.Empty;

                //panel

                if (capacity.Contains("w"))
                {
                    int panelWatt = product.CapacityValue;

                    double requiredKWh = (panelWatt * 5) / 1000.0; // 5 hours backup
                    double minKWh = requiredKWh * 0.5;
                    double maxKWh = requiredKWh * 2.0;

                    recomendations.Batteries = await GetRecommendedBatteries(requiredKWh);

                    //  Inverter calculation 
                    double requiredWatt = panelWatt * 1.3;
                    double minWatt = requiredWatt * 0.7;
                    double maxWatt = requiredWatt * 1.2;

                    recomendations.Inverters = await GetRecommendedInverters(minWatt, maxWatt);

                    recomendations.Message = $"Recommended setup for {panelWatt}W panel";
                }



                //Battery
                else if (capacity.Contains("ah") || capacity.Contains("kwh"))
                {
                    
                    double batteryKWh = ConvertBatteryToKWh(product);

                    double requiredWatt = batteryKWh * 1000;
                    recomendations.Inverters = await GetRecommendedInverters(requiredWatt * 0.8, requiredWatt * 1.5);

                    recomendations.Message = "Recommended inverters for your battery";
                }

                //  Inverter
                
                else if (capacity.Contains("va") || capacity.Contains("kw"))
                {
                    double inverterWatt = ConvertInverterToWatt(product);

                    double requiredKWh = inverterWatt / 1000.0;

                    recomendations.Batteries = await GetRecommendedBatteries(requiredKWh);

                    recomendations.Message = "Recommended battery for your inverter";
                }

                
                 // DEFAULT 
                
                else
                {
                   recomendations.Message = "Accessories recommended";
                }

                var prompt = BuildPrompt(recomendations, product);

                //recomendations.AIExplanation = await _aiInterfaceRepository.GenerateExplanation(prompt);

                recomendations.AIExplanation = "Ai Service is disabled";

                return recomendations;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private double ConvertBatteryToKWh(Products product)
        {
            var cap = product.Capacity?.ToLower() ??string.Empty;

            if (cap.Contains("ah"))
                return (product.CapacityValue * 12) / 1000.0;

            if (cap.Contains("kwh"))
                return product.CapacityValue;

            return 0;
        }

        private double ConvertInverterToWatt(Products product)
        {
            var cap = product.Capacity?.ToLower()??string.Empty;

            if (cap.Contains("va"))
                return product.CapacityValue * 0.8;

            if (cap.Contains("kw"))
                return product.CapacityValue * 1000;

            return 0;
        }


        private string BuildPrompt(RecommendationResponseDto data, Products product)
        {
            return $@"
            You are a solar expert AI.

            User selected:
            {product.ProductName} ({product.Capacity})

            Recommended:
            Batteries: {string.Join(", ", data.Batteries.Select(x => x.ProductName))}
            Inverters: {string.Join(", ", data.Inverters.Select(x => x.ProductName))}
            Accessories: {string.Join(", ", data.Accessories.Select(x => x.ProductName))}

            Explain in 2-3 lines why these are suitable.
            Use simple words.
            ";
        }

        public async Task<string> GetWhyThis(int mainProductId, int recommendedProductId)
        {
            var main = await _db.Products.FindAsync(mainProductId);
            var rec = await _db.Products.FindAsync(recommendedProductId);

            if(main == null || rec == null)
            {
                throw new NotFoundException(MessageHelper.Get("Products", "ProductId"));
            }

            var prompt = $@"
            User selected: {main.ProductName} ({main.Capacity})
            Recommended: {rec.ProductName} ({rec.Capacity})
             
            Explain why this is a good match in simple terms.
            ";

            return await _aiInterfaceRepository.GenerateExplanation(prompt);
        }

        public async Task<string> CompareProducts(int product1Id, int product2Id)
        {
            var p1 = await _db.Products.FindAsync(product1Id);
            var p2 = await _db.Products.FindAsync(product2Id);

            if (p1 == null || p2 == null)
            {
                throw new NotFoundException(MessageHelper.Get("Products", "ProductId"));
            }

                var prompt = $@"
                Compare these two solar products:

                Product 1: {p1.ProductName}, Capacity: {p1.Capacity}, Price: {p1.Price}
                Product 2: {p2.ProductName}, Capacity: {p2.Capacity}, Price: {p2.Price}

                Give:
                1. Key differences
                2. Which is better and why
                3. When to choose each
                Keep it simple and short.
                ";

            return await _aiInterfaceRepository.GenerateExplanation(prompt);
        }

        public async Task<ComboResposneDto> GetBestCombo(int productId)
        {
            var product = await _db.Set<Products>().Where(x => x.ProductId == productId).FirstOrDefaultAsync();
            if (product == null)
            {
                throw new NotFoundException(MessageHelper.Get("Poducts", "ProductId"));
            }

            var capacity = product.CapacityValue;
            double requiredKWh = (capacity * 5) / 1000.0;

            var batteries = await GetRecommendedBatteries(requiredKWh);

            double requiredWatt = capacity * 1.3;

            var inverters = await GetRecommendedInverters(requiredWatt * 0.7,requiredWatt * 1.5);

            var prompt = BuildBestComboPrompt(product, batteries, inverters);

            var aiResponse= await _aiInterfaceRepository.GenerateExplanation(prompt);
            logger.LogInformation(aiResponse);

            var expectedResult = JsonSerializer.Deserialize<ComboDto>(aiResponse.Trim(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            logger.LogInformation(expectedResult.BatteryName);
            logger.LogInformation(expectedResult.InverterName);

            var cleanInverterName = expectedResult.InverterName.Split("-")[0].Trim();

            var battery = await _db.Products.Include(x=>x.ProductImages).FirstOrDefaultAsync(x => x.ProductName.Contains(expectedResult.BatteryName));

            var inverter = await _db.Products.Include(x=>x.ProductImages)
                .FirstOrDefaultAsync(x => x.ProductName.Contains(cleanInverterName));
            if (inverter == null)
            {
                throw new NotFoundException(MessageHelper.Get("Products","NotFound"));
            }


            return new ComboResposneDto
            {
                Battery = MapToDto(battery),
                Inverter = MapToDto(inverter),
                Reason = expectedResult.Reason
            };
        }

        private ProductDto MapToDto(Products products)
        {
            if (products == null)
            {
                return null;
            }
            return new ProductDto
            {
                ProductId = products.ProductId,
                ProductName = products.ProductName,
                Stock = products.Stock,
                Price = products.Price,
                Description = products.Description,
                Warranty_Years = products.Warranty_Years,
                CapacityValue = products.CapacityValue,
                Capacity = products.Capacity,
                ProductImages = products.ProductImages.Select(x => new ProductImageResponseDto
                {
                    ProductId = x.ProductId,
                    ImageId = x.ImageId,
                    ImageUrl = x.ImageUrl,
                }).ToList()
            };
        }



        public static string BuildBestComboPrompt(Products panel, List<ProductDto> batteries, List<ProductDto> inverters)
        {
            return $@"
            User selected solar panel:
            {panel.ProductName} ({panel.Capacity})

            Available Batteries:
            {string.Join("\n", batteries.Select(b => $"{b.ProductName} - {b.Capacity}"))}

            Available Inverters:
            {string.Join("\n", inverters.Select(i => $"{i.ProductName} - {i.Capacity}"))}

             Return ONLY valid JSON. Do not include any text before or after.

            Format:
            {{
              ""batteryName"": ""..."",
              ""inverterName"": ""..."",
              ""reason"": ""...""
            }}
            ";
        }


        public static string BuildBudgetPrompt(Products panel, decimal budget,List<Products> batteries, List<Products> inverters)
        {
            return $@"
            User Budget: ₹{budget}
            Panel: {panel.ProductName}

            Available Batteries:
            {string.Join("\n", batteries.Select(b => $"{b.ProductName} - ₹{b.Price}"))}

            Available Inverters:
            {string.Join("\n", inverters.Select(i => $"{i.ProductName} - ₹{i.Price}"))}

            Suggest best combination within budget.
            Keep answer simple.";
        }

        public async Task<string> OptimizeBudget(int productId, decimal budget)
        {
            try
            {
                var product=await _db.Products.Where(x=>x.ProductId==productId).FirstOrDefaultAsync();
                if (product == null)
                {
                    throw new NotFoundException(MessageHelper.Get("Products", "ProductId"));
                }

                var batteriesParentCategoryId=await _db.Categories.Where(x=>x.Slug== "batteries").Select(x=>x.CategoryId).FirstAsync();

                var invertersParemtCategoryId = await _db.Categories.Where(x => x.Slug == "inverters").Select(x => x.CategoryId).FirstAsync();

                var batteries = await _db.Products.Include(x => x.Category).Where(x => x.Price <= budget && x.IsActive && x.Category.ParentCategoryId == batteriesParentCategoryId).ToListAsync();

                var inverters=await _db.Products.Include(x=>x.Category).Where(x=>x.Price<=budget&&x.IsActive&&x.Category.ParentCategoryId==invertersParemtCategoryId).ToListAsync();

                var result= BuildBudgetPrompt(product,budget, batteries, inverters);

                return  await _aiInterfaceRepository.GenerateExplanation(result);
            }
            catch(Exception)
            {
                throw;
            }
        }

        //public async Task<double> GetPredictionPower(PredictionRequest dto)
        //{
        //    try
        //    {

        //        if(dto == null) { throw new ArgumentNullException(nameof(dto)); }

        //        var result=await _aiInterfaceRepository.GetPredictPower(dto);
        //        double predictedResult = Math.Round(result, 2);
        //        return predictedResult;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}



        private PowerForecastResponse GenerateInsights(List<double> predictions,List<DateTime> forecastTimes)
        {
            var alerts = new List<string>();
            var hourlyData = new List<HourlyPrediction>();

            //  Filter only daytime (6 AM – 6 PM)
            var filtered = new List<(double power, DateTime time)>();

            for (int i = 0; i < predictions.Count; i++)
            {
                var time = forecastTimes[i];

                if(time.Hour >= 9 && time.Hour <= 17)
                {
                    filtered.Add((predictions[i], time));
                }
            }

            
            if (!filtered.Any())
            {
                throw new Exception("No daytime data available");
            }

            //  Extract filtered values
            var dayPredictions = filtered.Select(x => x.power).ToList();

            double avg = dayPredictions.Average();
            double peakPower = dayPredictions.Max();
            var peakItem = filtered.First(x => x.power == peakPower);

            //  Build hourly response
            foreach (var item in filtered)
            {
                hourlyData.Add(new HourlyPrediction
                {
                    Time = item.time.ToString("hh tt"),
                    Power = Math.Round(item.power, 2)
                });
            }



            // Low generation   
            if (avg < 4)
            {
                alerts.Add("Moderate solar generation expected");
            }

            // Very low
            if (avg < 3)
            {
                alerts.Add("Very low solar generation expected");
            }

            // Cloud impact (low power hours)
            if (dayPredictions.Any(p => p < 3.6))
            {
                alerts.Add("Cloud cover may reduce efficiency");
            }

            // Strong generation
            if (peakPower > 4.5)
            {
                alerts.Add("Strong solar generation expected during peak hours");
            }

            // Peak time 
            if (peakItem.time.Hour >= 9 && peakItem.time.Hour <= 17)
            {
                alerts.Add($"Peak generation expected at {peakItem.time:hh tt}");
            }
            else
            {
                alerts.Add("Peak generation expected during mid-day hours");
            }
            return new PowerForecastResponse
            {
                
                HourlyPredictions = hourlyData,
                AveragePower = Math.Round(avg, 2),
                PeakPower = Math.Round(peakPower, 2),
                PeakHour = peakItem.time.Hour, 
                Alerts = alerts
            };
        }

        public async Task<PowerForecastResponse> GetWhetherForecast(string userId)
        {
            try
            {
                //var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;

                //if (string.IsNullOrEmpty(userId))
                //{
                //    throw new Exception("User not authenticated");
                //}
                var result = await _aiInterfaceRepository.GetPredictPower();

                var response = GenerateInsights(result.Predictions, result.ForecastTimes);

                await SaveAlertsIntoDataBase(response.Alerts, userId);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task SaveAlertsIntoDataBase(List<string> alerts,string userId)
        {
            foreach(var alert in alerts)
            {
                var exist = await _db.Notifications.AnyAsync(x => x.Message == alert && x.CreatedAt.Date == DateTime.Today);
                if (!exist)
                {
                    var notification = new Notifications
                    {
                        UserId=userId,
                        Message=alert,
                        Type=NotificationType.Info,
                        CreatedAt=TimeUtility.NowIST(),

                    };

                    await _db.AddAsync(notification);

                    var signal = new NotificationDto
                    {
                        UserId = notification.UserId,
                        NotificationId = notification.NotificationId,
                        NotificationType = notification.Type.ToString()!,
                        Title = notification.Title,
                        Message = notification.Message,
                        IsRead = false,
                        CreatedAt = notification.CreatedAt,
                    };

                    await signalRRepository.SendAsyncNotification(notification.UserId, signal);
                }
            }

            await _db.SaveChangesAsync();
        }

        public async Task<List<double>> GetPredictPowerForOneWeek()
        {
            try
            {
                var result = await _aiInterfaceRepository.GetWeatherInputs();
                var predictions = new List<double>();
                foreach(var item in result)
                {
                    var power = await _aiInterfaceRepository.GetPredictedPower(new PredictionRequest
                    {
                        Irradiance = item.Irradiance,
                        Temperature = item.Temperature,
                        CloudCover = item.CloudCover,
                        Humidity = item.Humidity,
                        Hour = item.Hour
                    });

                    predictions.Add(power);
                }
                return predictions;
            }
            catch(Exception ex)
            {
                var errorMessages = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception(errorMessages);
            }
        }

        public async Task<List<DailyForecast>> GetDailyForecasts(string userId)
        {
            try
            {
                var weatherData=await _aiInterfaceRepository.GetDailyWeatherInputs();
                var grouped = weatherData.GroupBy(x => x.Time.Date);
                var list=new List<DailyForecast>();
                var capcityValue=await CalculateTotalCapacityOfUser(userId);
                foreach(var day in grouped)
                {
                    var task = day.Where(item => item.Hour >= 6 && item.Hour <= 18).Select(async item =>
                    {
                        var power = await _aiInterfaceRepository.GetPredictedPower(new PredictionRequest
                        {
                            Irradiance=item.Irradiance,
                            Temperature=item.Temperature,
                            CloudCover=item.CloudCover,
                            Humidity=item.Humidity,
                            Hour=item.Hour
                        });


                        return power * capcityValue;
                    });
                    var result=await Task.WhenAll(task);

                    list.Add(new DailyForecast
                    {
                        Date = day.Key.ToString("MMM dd"),
                        TotalEnergy = Math.Round(result.Sum(), 2)
                    });
                }
                return list;
            }   
            catch(Exception)
            {
                throw;
            }
        }

        private async Task<double> CalculateTotalCapacityOfUser(string userId)
        {
            double totalCapacity = 0;

            var activePanels = await _db.Panels.Where(x => x.UserId == userId).ToListAsync();

            foreach (var pan in activePanels)
            {
                if (!string.IsNullOrEmpty(pan.Capacity) && pan.Capacity.Contains("W"))
                {
                    var value = pan.Capacity.Split('W')[0].Trim();

                    if (double.TryParse(value, out double parsedValue))
                    {
                        totalCapacity += parsedValue;
                    }
                }
            }

            return totalCapacity / 1000;
        }
    }
}

   



