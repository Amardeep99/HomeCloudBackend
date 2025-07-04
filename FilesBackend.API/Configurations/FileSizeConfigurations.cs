using System.Net;
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

    private static class FileUploadSize
    {
        private static long Small { get; } = 10L * 1024 * 1024; // 10 MB
        private static long Medium { get; } = 100L * 1024 * 1024; // 100 MB
        private static long Large { get; } = 1L * 1024 * 1024 * 1024; // 1 GB
        private static long ExtraLarge { get; } = 15L * 1024 * 1024 * 1024; // 15 GB

        public static Dictionary<string, long> Limits { get; } = new()
        {
            { "Small", Small },
            { "Medium", Medium },
            { "Large", Large },
            { "ExtraLarge", ExtraLarge }
        };
    }
}