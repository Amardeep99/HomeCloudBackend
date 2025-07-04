using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FilesBackend.Controllers;

[ApiController]
[Route("files")]
public class FilesController : ControllerBase
{
    [HttpGet("{filename}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get([FromRoute] string filename)
    {
        var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Images", filename);

        if (!System.IO.File.Exists(filepath))
            return NotFound("File does not exist");

        var fileType = GetContentType(filepath);

        return PhysicalFile(filepath, fileType, filename);
    }

    [HttpGet("names")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAllNames()
    {
        var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Images");

        var filenames = Directory.GetFiles(filepath)
            .Select(Path.GetFileName)
            .ToList();

        return Ok(filenames);
    }

    [HttpPost]
    [RequestSizeLimit(15L * 1024 * 1024 * 1024)] // 15 GB
    [RequestFormLimits(MultipartBodyLengthLimit = 15L * 1024 * 1024 * 1024)] // For multipart/form-data specifically
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("File is empty or not provided");
        }

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
        var filePath = Path.Combine(folderPath, file.FileName);

        if (System.IO.File.Exists(filePath))
            return BadRequest("File already exists");

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        return Ok($"File uploaded successfully: {file.FileName}");
    }

    [HttpDelete("{filename}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Delete([FromRoute] string filename)
    {
        var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Images", filename);

        if (!System.IO.File.Exists(filepath))
            return NotFound("File does not exist");

        try
        {
            System.IO.File.Delete(filepath);
            return Ok("File deleted successfully");
        }
        catch (Exception)
        {
            return BadRequest("File does not exist");
        }
    }

    private string GetContentType(string path)
    {
        var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(path, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;
    }
}