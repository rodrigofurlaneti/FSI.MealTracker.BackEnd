using FSI.MealTracker.Application.Interfaces;
using FSI.MealTracker.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FSI.MealTracker.Application.DependencyInjection
{
    public static class ApplicationDependencyInjection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IConsumptionAppService, ConsumptionAppService>();
            services.AddScoped<IDailyGoalAppService, DailyGoalAppService>();
            services.AddScoped<IFoodAppService, FoodAppService>();
            services.AddScoped<IMealAppService, MealAppService>();
            services.AddScoped<IMealScheduleAppService, MealScheduleAppService>();
            services.AddScoped<IUserAppService, UserAppService>();
            services.AddScoped<IMessagingAppService, MessagingAppService>();
        }
    }
}