using FilesBackend.Database;
using Moq;

namespace FilesBackend.Services.Tests;

public class FilesServiceTests
{
    private readonly Mock<FilesDbContext> _mockContext;
    private readonly FilesService _service;

    public FilesServiceTests()
    {
        _mockContext = new Mock<FilesDbContext>();
        _service = new FilesService(_mockContext.Object);
    }
    
    [Fact]
    public void Test1()
    {
    }
}