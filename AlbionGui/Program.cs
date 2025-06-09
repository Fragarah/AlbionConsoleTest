// AlbionGui/Program.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using AlbionConsole.Data;
using AlbionConsole.Services;

namespace AlbionGui
{
    internal static class Program
    {
        public static IHost AppHost { get; private set; }

        [STAThread]
        static void Main()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseSqlite("Data Source=albion.db");
                        options.EnableSensitiveDataLogging();
                        options.EnableDetailedErrors();
                    });

                    services.AddTransient<DatabaseService>();
                    services.AddTransient<Form1>(); // dodajemy nasz formularz
                    services.AddTransient<ItemImporter>();
                    services.AddTransient<TrackedItemsForm>();
                })
                .Build();

            ApplicationConfiguration.Initialize();

            // Uruchamiamy formularz, który pobieramy z kontenera
            var form = AppHost.Services.GetRequiredService<Form1>();
            Application.Run(form);
            
        }
    }
}