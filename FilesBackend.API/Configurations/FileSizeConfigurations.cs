using System.Net;
using FilesBackend.Constants;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace FilesBackend.Configurations;

public static class FileSizeConfigurations
{
    private static IServiceCollection ConfigureFileUploadSize(this IServiceCollection services,
        long maxSizeBytes)
    {
        services.Configure<IISServerOptions>(options =>
        {
            options.MaxRequestBodySize = maxSizeBytes;
        });

        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = maxSizeBytes;
        });

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = maxSizeBytes;
        });

        return services;
    }

    public static IServiceCollection ConfigureFileSizeLimit(this IServiceCollection services, IConfiguration configuration)
    {
        var fileSizeFromConfig = configuration.GetValue<string>("FileSize");
        
        if (string.IsNullOrWhiteSpace(fileSizeFromConfig))
            throw new InvalidOperationException("Missing FileSize configuration");
        
        if (!FileUploadSize.Limits.TryGetValue(fileSizeFromConfig, out var fileSize))
        {
            var validOptions = string.Join(", ", FileUploadSize.Limits.Keys);
            throw new InvalidOperationException($"Invalid FileSize configuration: '{fileSizeFromConfig}'. Valid options are: {validOptions}");
        }

        services.ConfigureFileUploadSize(fileSize);
        
        return services;
    }
}