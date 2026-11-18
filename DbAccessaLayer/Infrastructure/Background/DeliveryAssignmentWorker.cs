using ApplicationLayer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure.Background
{
    public class DeliveryAssignmentWorker : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public DeliveryAssignmentWorker(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory= serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope= serviceScopeFactory.CreateScope();

                var deliveryRepository = scope.ServiceProvider.GetRequiredService<IDeliveryInterface>();

                await deliveryRepository.AssignedRemainingDeliveries();

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
