using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ApplicationLayer.Interfaces;
using DbAccessaLayer.Infrastructure;


public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
               
                services.AddHostedService<BackgroundServiceForDelayOrders>();
            })
            .Build();

        await host.RunAsync();
    }
}