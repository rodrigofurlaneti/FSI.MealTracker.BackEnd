using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FSI.MealTracker.Domain.Interfaces;
using FSI.MealTracker.Infrastructure.Context;
using FSI.MealTracker.Infrastructure.Messaging;
using FSI.MealTracker.Infrastructure.Repositories;
using FSI.MealTracker.Application.Interfaces;

namespace FSI.MealTracker.Infrastructure.DependencyInjection
{
    public static class InfrastructureDependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDbContext, DapperDbContext>();

            // Repositórios
            services.AddScoped<IConsumptionRepository, ConsumptionRepository>();
            services.AddScoped<IDailyGoalRepository, DailyGoalRepository>();
            services.AddScoped<IFoodRepository, FoodRepository>();
            services.AddScoped<IMealRepository, MealRepository>();
            services.AddScoped<IMealScheduleRepository, MealScheduleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMessagingRepository, MessagingRepository>();


            // RabbitMQ Publisher
            services.AddSingleton<IMessageQueuePublisher, RabbitMqPublisher>();
        }
    }
}
