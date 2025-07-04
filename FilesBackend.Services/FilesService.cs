using static System.IO.File;

namespace FilesBackend.Services;

public interface IFilesService
{
    string? GetFilepathIfFileExists(string filename);
    List<string> GetAllFilesNames();
    
    Stream GetStreamToAddFile();
    
    bool DeleteFile(string filename);
}

public class FilesService : IFilesService
{
    private readonly string _filepath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
    
    public string? GetFilepathIfFileExists(string filename)
    {
        var filepath = Path.Combine(_filepath, filename);
        
        return !File.Exists(filepath) ? null : filepath;
    }

    public List<string> GetAllFilesNames()
    {
        List<string> fileNames = new();
        
        fileNames.AddRange(Directory.GetFiles(_filepath));
        
        return fileNames;
    }

    public Stream GetStreamToAddFile()
    {
        return new FileStream(_filepath, FileMode.Create);
    }

    public bool DeleteFile(string filename)
    {
        var fileToDelete = Path.Combine(_filepath, filename);
        
        try
        {
            File.Delete(fileToDelete);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
}