using FilesBackend.Configurations;
using FilesBackend.Database.Extensions;
using FilesBackend.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace FilesBackend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.ConfigureFileSizeLimit(configuration);

        services.AddAuthorization();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        services.AddFilesDb(configuration);

        services.AddTransient<IFilesService, FilesService>();

        services.AddControllers();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAnyOrigin",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseCors("AllowAnyOrigin");
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}