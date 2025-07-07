using FilesBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static System.IO.File;

namespace FilesBackend.Controllers;

[ApiController]
[Route("files")]
public class FilesController(IFilesService filesService) : ControllerBase
{
    [HttpGet("exists/{filename}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckIfFileExists([FromRoute] string filename)
    {
        var file = await filesService.CheckFileExists(filename);
        
        return Ok(file);
    }

    [HttpGet("names")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllNames()
    {
        var filenames = await filesService.GetAllFilesNames(); 

        return Ok(filenames);
    }

    [HttpPost]
    [RequestSizeLimit(15L * 1024 * 1024 * 1024)] // 15 GB
    [RequestFormLimits(MultipartBodyLengthLimit = 15L * 1024 * 1024 * 1024)] // For multipart/form-data specifically
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> Post(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("File is empty or missing");
        }
        
        var exists = await filesService.CheckFileExists(file.FileName);

        if (exists)
            return Conflict("File already exists");
        
        await using var stream = file.OpenReadStream();

        try
        {
            await filesService.AddFile(file.FileName, stream, file.ContentType);
        }
        catch (NotSupportedException)
        {
            return BadRequest("File is not supported");
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
        
        return Ok();
    }

    [HttpDelete("{filename}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] string filename)
    {
        var exists = await filesService.CheckFileExists(filename);
        
        if (!exists)
            return NotFound("File does not exist");
        
        await filesService.DeleteFile(filename);
            
        return Ok("File deleted successfully");
    }
}