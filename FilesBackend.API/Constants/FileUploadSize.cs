namespace FilesBackend.Constants;

public static class FileUploadSize
{
    public static long Small { get; } = 10L * 1024 * 1024; // 10 MB
    public static long Medium { get; } = 100L * 1024 * 1024; // 100 MB
    public static long Large { get; } = 1L * 1024 * 1024 * 1024; // 1 GB
    public static long ExtraLarge { get; } = 15L * 1024 * 1024 * 1024; // 15 GB

    public static Dictionary<string, long> Limits { get; } = new()
    {
        { "Small", Small },
        { "Medium", Medium },
        { "Large", Large },
        { "ExtraLarge", ExtraLarge }
    };
}