using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure.Background
{
    public class AlertBackgroundService : BackgroundService
    {
        private IServiceScopeFactory serviceScopeFactory;

        public AlertBackgroundService(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
               using var scope=this.serviceScopeFactory.CreateScope();
               var adminRepository=scope.ServiceProvider.GetRequiredService<IAdminRepository>();
                var response = await adminRepository.GetProductionUpdateWithAI();
                var signalRepository = scope.ServiceProvider.GetRequiredService<ISignalRRepository>();
                var notificationRepository = scope.ServiceProvider.GetRequiredService<INoticationRepositry>();
                var requiredData = response.AnomalyDetails;

                if (requiredData.Any())
                {
                    foreach ( var item in requiredData)
                    {
                       await signalRepository.SendAlertsMessages(item);
                        await notificationRepository.SendAlertNotificationsForAdmins(item.PanelId,item.Severity,item.DropPercentage);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);
            }
        }
    }
}
