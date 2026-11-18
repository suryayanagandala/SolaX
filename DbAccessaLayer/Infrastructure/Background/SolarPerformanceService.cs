using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.DTO_S;
using CoreModels.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure.Background
{
    public class SolarPerformanceService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
       

        
        public SolarPerformanceService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
          
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var random = new Random();
                try
                {
                   
                    using var scope =  this.serviceProvider.CreateScope();

                    var unitOfWork=scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var _db=scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var signalRRepository = scope.ServiceProvider.GetRequiredService<ISignalRRepository>();

                    var result =await unitOfWork.solarPerformaceRepository.GetPanles();
                    foreach ( var user in result)
                    {


                        var data = new SolarPerformance
                        {
                            UserId = user.UserId,
                            PanelId = user.PanelId,
                            Voltage = random.Next(200, 240),
                            CurrentFlow = random.Next(5, 15),
                            Power = random.Next(1000, 2500),
                            Efficiency = random.Next(80, 98),
                            Temperature = random.Next(15,65),
                            Generated_at = TimeUtility.NowIST()

                        };
                        await _db.Set<SolarPerformance>().AddAsync(data);
                        var dto = new SolarPerformanceDto
                        {
                            UserId = user.UserId,
                            CurrentFlow = data.CurrentFlow,
                            PanelId=data.PanelId,
                            Power = data.Power,
                            Efficiency = data.Efficiency,
                            Voltage = data.Voltage,
                            Generated_at = data.Generated_at,
                            

                        };
                        await signalRRepository.SendNotificationForSolarPanles(user.UserId, dto);
                   
                    }

                    await _db.SaveChangesAsync();

                }
                catch {
                    throw;
                }
             await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);   
            }
        }
    }
}
