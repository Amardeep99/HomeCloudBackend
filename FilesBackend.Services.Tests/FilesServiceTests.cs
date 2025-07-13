using System.Text;
using FilesBackend.Database;
using FilesBackend.Database.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
namespace FilesBackend.Services.Tests;

public class FilesServiceTests : IDisposable
{
    private readonly FilesDbContext _context;
    private readonly FilesService _service;
    private readonly FileEntity _testFile;

    public FilesServiceTests()
    {
        var options = new DbContextOptionsBuilder<FilesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new FilesDbContext(options);
        _service = new FilesService(_context);

        _testFile = new FileEntity
        {
            FileName = "test.txt",
            ContentType = "text/plain",
            Content = Encoding.UTF8.GetBytes("test"),
            Size = 4
        };
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task CheckFileExists_WhenFileExists_ReturnsTrue()
    {
        // Arrange
        _context.Files.Add(_testFile);
        await _context.SaveChangesAsync();

        // Act 
        var result = await _service.CheckFileExists(_testFile.FileName);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CheckFileExists_WhenFileDoesNotExist_ReturnsFalse()
    {
        // Act
        var result = await _service.CheckFileExists(_testFile.FileName);

        // Assert
        result.Should().BeFalse();

    }

    [Fact]
    public async Task GetFile_WhenFileExists_ReturnsFile()
    {
        // Arrange
        _context.Files.Add(_testFile);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetFile(_testFile.FileName);

        // Assert
        result.Should().NotBeNull();
        result.FileName.Should().Be(_testFile.FileName);
        result.ContentType.Should().Be(_testFile.ContentType);
        result.Content.Should().BeEquivalentTo(_testFile.Content);
        result.Size.Should().Be(_testFile.Size);
    }

    [Fact]
    public async Task GetFile_WhenFileDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _service.GetFile(_testFile.FileName);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllFileNames_WhenFilesExist_ReturnsAllFileNames()
    {
        // Arrange
        var files = new List<FileEntity>
        {
            new() { FileName = "file1.txt", ContentType = "text/plain", Content = new byte[10], Size = 9 },
            new() { FileName = "file2.pdf", ContentType = "application/pdf", Content = new byte[10], Size = 9 },
            new() { FileName = "file3.jpg", ContentType = "image/jpeg", Content = new byte[10], Size = 9 },
        };

        _context.Files.AddRange(files);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllFilesNames();

        //
        result.Should().HaveCount(3);
        result.Should().Contain("file1.txt");
        result.Should().Contain("file2.pdf");
        result.Should().Contain("file3.jpg");
    }

    [Fact]
    public async Task GetAllFileNames_WhenNoFilesExist_IsEmptyList()
    {
        // Act
        var result = await _service.GetAllFilesNames();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllFileNames_WhenFileDoesNotExist_DoesNotContainFile()
    {
        // Arrange
        _context.Files.Add(_testFile);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllFilesNames();

        // Assert
        result.Should().NotContain("notExisting.txt");
    }

    [Fact]
    public async Task GetAllFilesMetadata_WhenFilesExist_ReturnsAllFileMetadata()
    {
        // Arrange
        _context.Files.Add(_testFile);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllFileMetadata();

        // Assert
        result.Should().Contain(f => f.FileName == _testFile.FileName);
        result.Should().Contain(f => f.ContentType == _testFile.ContentType);
        result.Should().Contain(f => f.Size == _testFile.Size);
        result.Should().Contain(f => f.CreatedAt == _testFile.CreatedAt);
        result.Should().Contain(f => f.ModifiedAt == _testFile.ModifiedAt);
    }

    [Fact]
    public async Task AddFile_AddsFile()
    {
        // Arrange
        await using var stream = new MemoryStream();
        await _service.AddFile(_testFile.FileName, stream, _testFile.ContentType);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Files.FirstOrDefaultAsync(f => f.FileName == _testFile.FileName);

        // Assert
        result.Should().NotBeNull();
        result.FileName.Should().Be(_testFile.FileName);
        result.ContentType.Should().Be(_testFile.ContentType);
    }

    [Fact]
    public async Task DeleteFile_DeletesFile()
    {
        // Arrange
        _context.Files.Add(_testFile);
        await _context.SaveChangesAsync();
        
        // Act
        await _service.DeleteFile(_testFile.FileName);
        var result = await _context.Files.FirstOrDefaultAsync(f => f.FileName == _testFile.FileName);
        
        // Assert
        result.Should().BeNull();
    }

}