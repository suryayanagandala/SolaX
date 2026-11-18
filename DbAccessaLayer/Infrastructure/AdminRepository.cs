using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _db;

        public AdminRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<AdminDashboardStatsDto> GetStats()
        {
            return new AdminDashboardStatsDto
            {
                Users = await _db.Users.CountAsync(),

                Employees = await _db.Users.CountAsync(x => x.RoleId == (int)RoleIds.Technician),

                Orders = await _db.Orders.CountAsync(),

                Products = await _db.Products.CountAsync(),

                TotalEmployess = await _db.Technicians.Where(x => x.IsActive).CountAsync(),

                PanlesCount = await _db.Panels.Where(x => x.IsActive).CountAsync(),

                Installation = await _db.Installations.Where(x => x.Status == InstallationStatus.Completed.ToString()).CountAsync(),

                Delieveries = await _db.Deliveries.Where(x => x.Status == DeliveryStatus.Delivered.ToString()).CountAsync()
            };

        }

        public async Task<List<UsersDataDto>> GetAllUsers()
        {
            try
            {
                var customersData = await _db.Users.Include(x=>x.Role).Where(x=>x.IsActive).OrderByDescending(x => x.CreatedDate).ToListAsync();
                var list = new List<UsersDataDto>();
                foreach (var item in customersData)
                {
                    list.Add(new UsersDataDto
                    {
                        Name=item.FullName,
                        Email=item.EmailId,
                        PhoneNumber=item.PhoneNumber,
                        RoleName=item.Role.RoleName,
                        UserId=item.UserId,
                        ActionStatus = item.RoleId == 2 ? "Action" : ""
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> GetGlobalProduction()
        {
            try
            {
                var now = TimeUtility.NowIST();

                var currentYear = now.Year;
                var currentMonth = now.Month;

                var startOfYear = new DateTime(currentYear, 1, 1);
                var endOfYear = startOfYear.AddYears(1);

                var startOfDay = now.Date;
                var endOfDay = startOfDay.AddDays(1);

                var previousMonthDate = now.AddMonths(-1);
                var previousMonth = previousMonthDate.Month;

           
                var yearlyData = await _db.SolarPerformances
                    .Where(x => x.Generated_at >= startOfYear &&
                                x.Generated_at < endOfYear)
                    .ToListAsync();  

               
                var monthlyTrend = yearlyData
                    .GroupBy(x => x.Generated_at.Month)
                    .Select(k => new
                    {
                        Month = k.Key,
                        TotalPower = k.Sum(x => x.Power)
                    })
                    .OrderBy(x => x.Month)
                    .ToList();

                var todayData = await _db.SolarPerformances
                    .Where(x => x.Generated_at >= startOfDay &&
                                x.Generated_at < endOfDay)
                    .ToListAsync();   

               
                var dailyTrend = todayData
                    .GroupBy(x => x.Generated_at.Hour)
                    .Select(x => new
                    {
                        Hour = x.Key,
                        TotalPower = x.Sum(x => x.Power)
                    })
                    .OrderBy(x => x.Hour)
                    .ToList();

                var currentMonthTotal = monthlyTrend
                    .Where(x => x.Month == currentMonth)
                    .Select(x => x.TotalPower)
                    .FirstOrDefault();

                var previousMonthTotal = monthlyTrend.Where(x => x.Month == previousMonth).Select(x => x.TotalPower)
                               .FirstOrDefault();

                decimal growthPercent = 0m;

                if (previousMonthTotal > 0)
                {
                    growthPercent =(decimal)((currentMonthTotal - previousMonthTotal)/ previousMonthTotal) * 100m;
                }

                return new
                {
                    monthlyTrend,
                    dailyTrend,
                    changePercent = growthPercent
                };
            }
            catch
            {
                throw;
            }
        }

        public async Task<object> ProductionTrend()
        {
            try
            {
                var now = TimeUtility.NowIST();

                var currentMonthStart = new DateOnly(now.Year, now.Month, 1);
                var nextMonthStart = currentMonthStart.AddMonths(1);

                var previousMonthStart = currentMonthStart.AddMonths(-1);
                var previousMonthEnd = currentMonthStart;

               
                var currentMonthData = await _db.OrderItems
                    .Where(x => x.Order.Status == OrderStatus.DELIVERED &&
                                x.Order.OrderDate >= currentMonthStart &&
                                x.Order.OrderDate < nextMonthStart)
                    .Include(x => x.Product)
                        .ThenInclude(p => p.ProductImages)
                    .ToListAsync(); 

                var previousMonthData = await _db.OrderItems
                    .Where(x => x.Order.Status == OrderStatus.DELIVERED &&
                                x.Order.OrderDate >= previousMonthStart &&
                                x.Order.OrderDate < previousMonthEnd)
                    .Include(x => x.Product)
                        .ThenInclude(p => p.ProductImages)
                    .ToListAsync(); 

              
                var currentSales = currentMonthData
                    .GroupBy(x => new
                    {
                        x.ProductId,
                        x.Product.ProductName,
                        ImageUrl = x.Product.ProductImages.FirstOrDefault()?.ImageUrl
                    })
                    .Select(g => new
                    {
                        g.Key.ProductId,
                        g.Key.ProductName,
                        g.Key.ImageUrl,
                        TotalQuantity = g.Sum(x => x.Quantity)
                    })
                    .ToList();

                var previousSales = previousMonthData
                    .GroupBy(x => new
                    {
                        x.ProductId,
                        x.Product.ProductName,
                        ImageUrl = x.Product.ProductImages.FirstOrDefault()?.ImageUrl
                    })
                    .Select(g => new
                    {
                        g.Key.ProductId,
                        TotalQuantity = g.Sum(x => x.Quantity)
                    })
                    .ToList();

                var result = currentSales.Select(current =>
                {
                    var previous = previousSales
                        .FirstOrDefault(p => p.ProductId == current.ProductId);

                    double percent = 0;

                    if (previous == null || previous.TotalQuantity == 0)
                    {
                        percent = current.TotalQuantity > 0 ? 100 : 0;
                    }
                    else
                    {
                        percent = ((double)(current.TotalQuantity - previous.TotalQuantity)
                                  / previous.TotalQuantity) * 100;
                    }

                    return new
                    {
                        current.ImageUrl,
                        current.ProductId,
                        current.ProductName,
                        current.TotalQuantity,
                        changePercentage = percent
                    };
                })
                .OrderByDescending(x => x.changePercentage)
                .ToList();

                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<object> GetProductionUpdate()
        {
            try
            {
                var now = TimeUtility.NowIST();
                var startOfDay = now.Date;
                var endOfDay = startOfDay.AddDays(1);

             
                var offlinePanels = await _db.SolarPerformances
                    .Include(x => x.Panel)
                    .Where(x => x.Generated_at >= startOfDay &&
                                x.Generated_at < endOfDay &&
                                x.PanelId != x.Panel.PanelId)
                    .CountAsync();

               
                var activePerformances = await _db.SolarPerformances
                    .Where(x => x.IsActive)
                    .ToListAsync();

                var lowProductionPanels = activePerformances
                    .GroupBy(x => x.PanelId)  
                    .Select(g => new
                    {
                        PanelId = g.Key,
                        TotalPower = g.Sum(x => x.Power)
                    })
                    .Count(x => x.TotalPower < 500);

              
                var lowProductStock = await _db.Products
                    .Where(x => x.Stock <= 20 && x.IsActive)
                    .Select(x => new
                    {
                        x.ProductId,
                        x.ProductName,
                        x.Stock
                    }).Take(5).ToListAsync();

                return new
                {
                    offlinePanels,
                    lowProductionPanels,
                    lowProductStock
                };
            }
            catch
            {
                throw;
            }
        }


        public async Task<List<object>> SoldProducts()
        {
            try
            {
                var soldProducts = await _db.OrderItems.Where(x => x.Order.Status == OrderStatus.DELIVERED).
                                 GroupBy(x => new
                                 {
                                     x.ProductId,
                                     x.Product.ProductName,
                                     x.Product.ProductImages.First().ImageUrl,
                                 }).Select(x => new
                                 {
                                     x.Key.ProductId,
                                     x.Key.ProductName,
                                     x.Key.ImageUrl,
                                     Quantity = x.Sum(x => x.Quantity)
                                 }).ToListAsync();
                return new List<object>
                {
                    soldProducts
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ProductionUpdateWithAiModel> GetProductionUpdateWithAI()
        {
            try
            {
                var now = TimeUtility.NowIST();
                var startOffDay = now.Date;
                var endOffDay= now.AddDays(1);
                bool isAnomaly;

                //offlinePanels
                var offLinePanles = await _db.SolarPerformances.Include(x => x.Panel).Where(x => x.Generated_at >= startOffDay && x.Generated_at <= endOffDay
                                   && x.PanelId != x.Panel.PanelId).CountAsync();

                var latestPerformances = await _db.SolarPerformances.Where(x => x.Generated_at >= startOffDay.AddDays(-7)).ToListAsync();
                var panelAnomalies = latestPerformances.GroupBy(x => x.PanelId)
            .Select(g =>
            {
                var orderedDate = g.OrderBy(x => x.Generated_at).ToList();
                var values = orderedDate.Select(x => x.Power).ToList();

                if (values.Count < 5) return null;

                var avg = values.Average();
                var stdDev = Math.Sqrt(values.Average(v => Math.Pow((double)(v - avg), 2)));

                var latestRecord = orderedDate.Last().Power;

                bool isAnomaly = false;
                double avgDouble = 0;
                double latestDouble = 0;

                if (avg.HasValue && latestRecord.HasValue)
                {
                    avgDouble = (double)avg.Value;
                    latestDouble = (double)latestRecord.Value;
                    isAnomaly = latestDouble < (avgDouble - 1.5 * stdDev);
                }

                //string severity = "Normal";

                //if (isAnomaly)
                //{
                //    if (latestDouble < avgDouble - 3 * stdDev)
                //        severity = "High";
                //    else if (latestDouble < avgDouble - 2.5 * stdDev)
                //        severity = "Medium";
                //    else
                //        severity = "Low";
                //}

                double dropPercent =(double) ((avg - latestRecord) / avg) * 100;

                string severity = "Normal";

                if (isAnomaly)
                {
                    if (dropPercent > 40)
                        severity = "High";
                    else if (dropPercent > 25)
                        severity = "Medium";
                    else
                        severity = "Low";
                }

                return new PanelAnomalyDetail
                {
                    PanelId = g.Key,
                    IsAnomaly = isAnomaly,
                    Severity = severity,
                    LatestPower = latestRecord,
                    Deviation = Math.Round(avgDouble - latestDouble, 2),
                    DropPercentage = (double)Math.Round(dropPercent, 2),
                    AvgPower = Math.Round(avgDouble, 2),
                };
            }).Where(x => x != null && x.IsAnomaly).OrderByDescending(x => x.Severity).ToList();

                var lowProductStock = await _db.Products
                   .Where(x => x.Stock <= 20 && x.IsActive)
                   .Select(x => new LowStockProduct
                   {
                      ProductId=  x.ProductId,
                      ProductName= x.ProductName,
                      Stock=  x.Stock
                   }).Take(5).ToListAsync();
                return new ProductionUpdateWithAiModel
                {
                    OffLinePanels = offLinePanles,
                    AIAnomalyPanels = panelAnomalies.Count,
                    AnomalyDetails = panelAnomalies,
                    LowProductStock = lowProductStock
                };

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
