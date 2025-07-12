using FilesBackend.Database.Models;

namespace FilesBackend.Services.Dto;

public class FileMetadata
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public static FileMetadata Map(FileEntity fileEntity)
    {
        return new FileMetadata()
        {
            FileName = fileEntity.FileName,
            ContentType = fileEntity.ContentType,
            Size = fileEntity.Size,
            CreatedAt = fileEntity.CreatedAt,
            ModifiedAt = fileEntity.ModifiedAt
        };
    }
}