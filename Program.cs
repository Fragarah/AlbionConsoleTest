using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AlbionConsole.Services;
using AlbionConsole.Data;

Console.WriteLine("Starting application...");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddLogging(configure => configure.AddDebug());
        
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite("Data Source=albion.db");
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });

        services.AddTransient<DatabaseService>();
        services.AddTransient<AlbionApiService>();
        services.AddTransient<ItemImporter>();
        services.AddTransient<MailNotificationService>();
        services.AddTransient<ConsoleService>();
    })
    .Build();

try 
{
    Console.WriteLine("Starting ConsoleService...");
    var consoleService = host.Services.GetRequiredService<ConsoleService>();
    await consoleService.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Error during startup: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    throw;
} 