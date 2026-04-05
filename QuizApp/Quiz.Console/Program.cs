using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quiz.DataAccess;
using Quiz.DataAccess.Models;
using Quiz.DataAccess.Services;

namespace Quiz.Console
{
    internal class Program
    {
        static async Task Main(string[] args) {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((_, config) => {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddUserSecrets<Program>();
                })
                .ConfigureServices((hostContext, services) => {
                    services.AddDataAccess(hostContext.Configuration);
                }).Build();

            host.Start();
            
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<QuizAppDbContext>();
                
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                
                await DbInitializer.Initialize(context, roleManager, userManager);
            }



        }

    }

}
