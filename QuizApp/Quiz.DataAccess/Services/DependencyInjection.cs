using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Quiz.DataAccess.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration config)
    {   
        var connectionString = config.GetConnectionString("DefaultConnection");
        services.AddDbContext<QuizAppDbContext>(options => options
            .UseSqlServer(connectionString)
        );
        
        return services;
    }
}