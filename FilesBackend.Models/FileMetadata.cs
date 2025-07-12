namespace FilesBackend.Models;

public class FileMetadata
{
    public string Name { get; set; }
    public string ContentType { get; set; }
    public long Length { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}