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
    public class WeatherForeCastSerice : BackgroundService
    {
       private readonly IServiceScopeFactory serviceScopeFactory;
        public WeatherForeCastSerice(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope= serviceScopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var accessoryInterface = unitOfWork.accessRepository;
                var rolesRepository = unitOfWork.RoleRepository;
                var allUsers=rolesRepository.GetAllUsers();
                foreach(var user in allUsers)
                {

                    await accessoryInterface.GetWhetherForecast(user.UserId);
                }
                
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
