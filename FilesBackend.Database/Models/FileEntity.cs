using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FilesBackend.Database.Models;

public class FileEntity
{
    [Key]
    public Guid Id{ get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FileName { get; set; }
    
    [Required]
    public byte[] Content { get; set; }
    
    [MaxLength(100)]
    public string ContentType { get; set; }
    
    public long Size { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime ModifiedAt { get; set; }
}