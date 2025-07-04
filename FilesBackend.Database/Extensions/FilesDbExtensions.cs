using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FilesBackend.Database.Extensions;
public static class FilesDbExtensions
{
    public static IServiceCollection AddFilesDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FilesDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}