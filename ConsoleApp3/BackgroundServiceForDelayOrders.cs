using System.ComponentModel;
using ApplicationLayer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
public class BackgroundServiceForDelayOrders : BackgroundService
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    public BackgroundServiceForDelayOrders(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope= this.serviceScopeFactory.CreateScope();
            var deliveryRepository = scope.ServiceProvider.GetRequiredService<IDeliveryInterface>();
            await deliveryRepository.SentDelayMessagesForDeliveryData();
            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }
}
