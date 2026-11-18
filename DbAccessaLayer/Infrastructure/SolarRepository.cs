using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using CoreModels.Entities;
using CoreModels.Enums;
using DbAccessaLayer.Helpers;
using iText.Kernel.Pdf.Tagutils;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure
{
    public class SolarRepository : ISolarPerformaceRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly AssignmentSettings assignmentSettings;

        public SolarRepository(ApplicationDbContext db,IOptions<AssignmentSettings> options)
        {
            _db = db;
            assignmentSettings = options.Value;
        }
        public async Task<IEnumerable<SolarPeformance>> GetAllUsers()
        {
            var users = await _db.Users
                .Include(u => u.Order)
                    .ThenInclude(o => o.OrderItem)
                        .ThenInclude(oi => oi.Product)
                .Include(u => u.Order)
                    .ThenInclude(o => o.Delivery)
                        .ThenInclude(d => d.Installations)
                .ToListAsync();

            var result = users
                .SelectMany(u => u.Order
                    .Where(o => o.IsActive &&
                                o.OrderItem.Any() &&
                                o.Delivery != null &&
                                o.Delivery.Status == DeliveryStatus.Delivered.ToString() &&
                                o.Delivery.Installations.Any(i =>
                                    i.Status == InstallationStatus.Completed.ToString()))
                    .SelectMany(o => o.OrderItem.Select(item => new SolarPeformance
                    {
                        UserId = u.UserId,
                        UserName = u.FullName,
                        OrderId = o.OrderId,
                        DeliveryStatus = o.Delivery!.Status!,
                        InstallationStatus = o.Delivery.Installations
                            .FirstOrDefault(i => i.Status == InstallationStatus.Completed.ToString())?.Status
                            ?? "No Installation",
                        ProductName = item.Product.ProductName,
                        Capacity = item.Product.Capacity
                    })))
                .OrderByDescending(x => x.OrderId)
                .ToList();

            return result;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummary(string userId, int? panelId=null)
        {
            try
            {
                var today=TimeUtility.StartOffDay();
                var monthDate=TimeUtility.StartOffCurrentMonth();

                var query =  _db.SolarPerformances.Where(x => x.UserId == userId);

                if (panelId.HasValue)
                {
                    query = query.Where(x => x.PanelId == panelId);
                }

                var data = await query.ToListAsync();

                var todayTotal = data.Where(x => x.Generated_at >= today).Sum(x => x.Power);

                var monthlyPower = data.Where(x => x.Generated_at >= monthDate).Sum(x => x.Power);

                var lifeTimePower = data.Sum(x => x.Power);

                var averageEfficiency = data.Average(x => x.Efficiency);

                var lifeTimeSavings = lifeTimePower * assignmentSettings.ChargePerUnit;
                return new DashboardSummaryDto
                {
                    TodayTotal = todayTotal,
                    MonthlyPower = monthlyPower,
                    LifeTimePower = lifeTimePower,
                    LifeTimeSavings = lifeTimeSavings,
                    AverageEfficiency = averageEfficiency
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<MonthlyProductionDto>> GetMonthlyProductionUpdate(string userId, int? panelId = null)
        {
            var startDate = TimeUtility.StartOffCurrentMonth();
            var endDate = startDate.AddMonths(1);

            var query = _db.SolarPerformances
                .Where(x => x.UserId == userId &&
                            x.Generated_at >= startDate &&
                            x.Generated_at < endDate);

            if (panelId.HasValue)
            {
                query = query.Where(x => x.PanelId == panelId.Value);
            }

            var data = await query
                .OrderBy(x => x.Generated_at)
                .ToListAsync();

            var result = data
                .GroupBy(x => x.Generated_at.Date)
                .Select(group =>
                {
                    double totalEnergy = 0;

                    var ordered = group.OrderBy(x => x.Generated_at).ToList();

                    for (int i = 1; i < ordered.Count; i++)
                    {
                        var prev = ordered[i - 1];
                        var curr = ordered[i];

                        var timeDiffHours = (curr.Generated_at - prev.Generated_at).TotalHours;

                        totalEnergy +=(double) prev.Power * timeDiffHours;
                    }

                    return new MonthlyProductionDto
                    {
                        Date = group.Key,
                        TotalPower =(decimal) totalEnergy 
                    };
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        public async Task<List<SolarPerformance>> GetPerformaces(string userId, int? panelId=null)
        {
            try
            {
                var query = _db.SolarPerformances.Where(x => x.UserId == userId);
                var last30Min = TimeUtility.NowIST().AddMinutes(-30);

                if (panelId.HasValue)
                {
                  query = query.Where(x=>x.PanelId==panelId);
                }
                var performance = await query.Where(x => x.IsActive && x.Generated_at >= last30Min).OrderBy(x => x.Generated_at) .ToListAsync();
                if ( performance.Count==0||!performance.Any())
                {
                    throw new NotFoundException(MessageHelper.Get("SolarPerformance", "NotFound"));
                }
                var results = new List<SolarPerformance>(); 
                foreach (var item in performance) 
                { 
                    results.Add(new SolarPerformance 
                    {
                          UserId = userId,
                          PanelId=item.PanelId,
                          Generated_at = item.Generated_at, 
                          Voltage = item.Voltage, 
                          CurrentFlow = item.CurrentFlow, 
                          Efficiency = item.Efficiency, 
                          Power = item.Power, 
                          PerformanceId = item.PerformanceId, 
                    }); 
                }
                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddToPanel()
        {
            try
            {
                var deliveries = await _db.Deliveries.Include(d => d.Order) .ThenInclude(o => o.OrderItem).ThenInclude(oi => oi.Product)
                                .Include(d => d.Installations).Where(d => d.Status == DeliveryStatus.Delivered.ToString()&& d.Installations.Any(i =>
                                i.Status == InstallationStatus.Completed.ToString() && i.IsPanelCreated==false)).ToListAsync();

                var panelsToAdd = deliveries
                    .SelectMany(d => d.Order.OrderItem.Select(oi => new Panel
                    {
                        UserId = d.Order.UserId,   
                        PanelName = oi.Product.ProductName,
                        Capacity = oi.Product.Capacity,
                        IsActive=true
                    }))
                    .ToList();

                await _db.Panels.AddRangeAsync(panelsToAdd);
                foreach (var delivery in deliveries)
                {
                    foreach (var installation in delivery.Installations
                             .Where(i => i.Status == InstallationStatus.Completed.ToString()))
                    {
                        installation.IsPanelCreated = true;
                    }
                }
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Panel>> GetPanles()
        {
            try
            {
                return await _db.Panels.Where(x => x.IsActive).ToListAsync();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<object>> GetActivatedPanles(string userId)
        {
            try
            {
                var panles = await _db.Panels.Where(x => x.UserId == userId && x.IsActive).
                                 Select(x => new
                                 {
                                     panelId = x.PanelId,
                                     PanelName = x.PanelName,
                                     Capacity = x.Capacity,
                                 }).ToListAsync<object>();
                return panles;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<object> GetPanelDetails(int panelId)
        {
            try
            {
                var panelDetails = await _db.SolarPerformances
                    .Where(x => x.PanelId == panelId && x.IsActive)
                    .OrderByDescending(x => x.Generated_at)
                    .Select(x => new
                    {
                        panelId = x.PanelId,
                        CurrentFlow = x.CurrentFlow,
                        Voltage = x.Voltage,
                        Efficiency = x.Efficiency,
                        Temperature = x.Temperature,
                        Power=x.Power,
                    }).FirstOrDefaultAsync();

                if (panelDetails == null)
                    return new { Message = "No active panel found" };

                return panelDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
