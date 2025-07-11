using FilesBackend.Database;
using FilesBackend.Database.Models;
using Microsoft.EntityFrameworkCore;


namespace FilesBackend.Services;

public interface IFilesService
{
    Task<bool> CheckFileExists(string filename);
    public Task<FileEntity?> GetFile(string filename);
    Task<List<string>>GetAllFilesNames();
    
    Task AddFile(string filename, Stream fileStream, string contentType);
    Task DeleteFile(string filename);
}

public class FilesService(FilesDbContext context) : IFilesService
{
    private readonly string _filepath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
    
    public async Task<bool> CheckFileExists(string filename)
    {
        return await context.Files.AnyAsync(x => x.FileName == filename);
    }

    public async Task<FileEntity?> GetFile(string filename)
    {
        var fileEntity = await context.Files.FirstOrDefaultAsync(x => x.FileName == filename);

        return fileEntity;
    }

    public async Task<List<string>> GetAllFilesNames()
    {
        return await context.Files.Select(f => f.FileName).ToListAsync();
    }

    public async Task AddFile(string filename, Stream fileStream, string contentType)
    {
        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        var content = memoryStream.ToArray();

        var fileEntity = new FileEntity
        {
            FileName = filename,
            Content = content,
            ContentType = contentType,
            Size = content.Length
        };
        
        context.Files.Add(fileEntity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteFile(string filename)
    {
        context.Files.Remove(new FileEntity { FileName = filename });
        await context.SaveChangesAsync();
    }
}