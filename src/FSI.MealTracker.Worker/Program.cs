using FSI.MealTracker.Application.DependencyInjection;
using FSI.MealTracker.Infrastructure.DependencyInjection;
using FSI.MealTracker.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        var env = context.HostingEnvironment;

        config.SetBasePath(AppContext.BaseDirectory); // <- FORÇA o diretório base correto

        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var conn = context.Configuration.GetConnectionString("DefaultConnection");
        Console.WriteLine($"🔗 Connection String: {conn}");

        services.AddHostedService<ConsumptionConsumer>();
        services.AddHostedService<DailyGoalConsumer>();
        services.AddHostedService<FoodConsumer>();
        services.AddHostedService<MealConsumer>();
        services.AddHostedService<MealScheduleConsumer>();
        services.AddHostedService<UserConsumer>();
        services.AddApplicationServices(); // camada Application
        services.AddInfrastructure(context.Configuration); // camada Infrastructure
    })
    .Build();

host.Run();