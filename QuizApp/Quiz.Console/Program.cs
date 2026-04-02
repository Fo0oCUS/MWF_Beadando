using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quiz.DataAccess.Services;

namespace Quiz.Console
{
    using Quiz.DataAccess.Models.Enums;
    using Quiz.DataAccess.Models;
    using System;
    using Microsoft.EntityFrameworkCore;

    internal class Program
    {
        static void Main(string[] args) {
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
                DbInitializer.Initialize(context);
            }



        }

    }

}
