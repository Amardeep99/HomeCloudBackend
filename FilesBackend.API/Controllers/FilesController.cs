using FilesBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static System.IO.File;

namespace FilesBackend.Controllers;

[ApiController]
[Route("files")]
public class FilesController(IFilesService filesService) : ControllerBase
{
    [HttpGet("{filename}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get([FromRoute] string filename)
    {
        var filePath = filesService.GetFilepathIfFileExists(filename);
        
        if(filePath == null)
            return NotFound("File not found");

        return PhysicalFile(filePath, filename);
    }

    [HttpGet("names")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAllNames()
    {
        var filenames = filesService.GetAllFilesNames(); 

        return Ok(filenames);
    }

    [HttpPost]
    [RequestSizeLimit(15L * 1024 * 1024 * 1024)] // 15 GB
    [RequestFormLimits(MultipartBodyLengthLimit = 15L * 1024 * 1024 * 1024)] // For multipart/form-data specifically
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> Post(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("File is empty or not provided");
        }

        if(filesService.GetFilepathIfFileExists(file.FileName) != null)
            return BadRequest($"File {file.FileName} already exists");

        await using var stream = filesService.GetStreamToAddFile();
        
        try
        {
            await file.CopyToAsync(stream);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        return Ok($"File uploaded successfully: {file.FileName}");
    }

    [HttpDelete("{filename}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Delete([FromRoute] string filename)
    {
        if (filesService.GetFilepathIfFileExists(filename) == null)
            return NotFound("File does not exist");

        var success = filesService.DeleteFile(filename);
        
        return success ? Ok("File deleted successfully") : StatusCode(StatusCodes.Status500InternalServerError);
    }
}