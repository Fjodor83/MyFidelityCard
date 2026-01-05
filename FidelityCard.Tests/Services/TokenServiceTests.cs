using FidelityCard.Srv.Repositories;
using FidelityCard.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace FidelityCard.Tests.Services;

public class FileTokenRepositoryTests : IDisposable
{
    private readonly Mock<IWebHostEnvironment> _mockEnv;
    private readonly FileTokenRepository _tokenRepository;
    private readonly string _testPath;

    public FileTokenRepositoryTests()
    {
        _mockEnv = new Mock<IWebHostEnvironment>();
        
        // Setup a temporary directory for tests
        _testPath = Path.Combine(Path.GetTempPath(), "FidelityCardTests_" + Guid.NewGuid());
        Directory.CreateDirectory(_testPath);
        
        _mockEnv.Setup(e => e.ContentRootPath).Returns(_testPath);

        _tokenRepository = new FileTokenRepository(_mockEnv.Object);
    }

    [Fact]
    public void GenerateToken_ShouldCreateTokenFile()
    {
        // Arrange
        string email = "test@example.com";
        string store = "NE001";

        // Act
        string token = _tokenRepository.GenerateToken(email, store);

        // Assert
        Assert.False(string.IsNullOrEmpty(token));
        
        string tokenPath = Path.Combine(_testPath, "Token", token);
        Assert.True(File.Exists(tokenPath));
        
        string content = File.ReadAllText(tokenPath);
        Assert.Contains(email, content);
        Assert.Contains(store, content);
    }

    [Fact]
    public async Task GetTokenDataAsync_ShouldReturnTokenData_WhenTokenExists()
    {
        // Arrange
        string email = "test@example.com";
        string store = "NE001";
        string token = "test-token-123";
        
        string tokenDir = Path.Combine(_testPath, "Token");
        Directory.CreateDirectory(tokenDir);
        string tokenPath = Path.Combine(tokenDir, token);
        await File.WriteAllTextAsync(tokenPath, $"{store}\r\n{email}");

        // Act
        TokenData? tokenData = await _tokenRepository.GetTokenDataAsync(token);

        // Assert
        Assert.NotNull(tokenData);
        Assert.Equal(store, tokenData.Store);
        Assert.Equal(email, tokenData.Email);
    }

    [Fact]
    public async Task GetTokenDataAsync_ShouldReturnNull_WhenTokenDoesNotExist()
    {
        // Act
        TokenData? tokenData = await _tokenRepository.GetTokenDataAsync("non-existent-token");

        // Assert
        Assert.Null(tokenData);
    }

    [Fact]
    public async Task ValidateTokenAsync_ShouldReturnSameAsGetTokenData()
    {
        // Arrange
        string email = "validate@example.com";
        string store = "NE002";
        string token = _tokenRepository.GenerateToken(email, store);

        // Act
        var tokenData = await _tokenRepository.ValidateTokenAsync(token);

        // Assert
        Assert.NotNull(tokenData);
        Assert.Equal(store, tokenData.Store);
        Assert.Equal(email, tokenData.Email);
    }

    [Fact]
    public void CleanupExpiredTokens_ShouldRemoveOldTokens()
    {
        // Arrange
        string tokenDir = Path.Combine(_testPath, "Token");
        Directory.CreateDirectory(tokenDir);
        
        // Create an old token file
        string oldTokenPath = Path.Combine(tokenDir, "old-token");
        File.WriteAllText(oldTokenPath, "NE001\r\nold@example.com");
        File.SetCreationTime(oldTokenPath, DateTime.Now.AddMinutes(-20));

        // Create a recent token file
        string recentTokenPath = Path.Combine(tokenDir, "recent-token");
        File.WriteAllText(recentTokenPath, "NE001\r\nrecent@example.com");

        // Act
        _tokenRepository.CleanupExpiredTokens(TimeSpan.FromMinutes(15));

        // Assert
        Assert.False(File.Exists(oldTokenPath), "Old token should be deleted");
        Assert.True(File.Exists(recentTokenPath), "Recent token should remain");
    }

    // Cleanup
    public void Dispose()
    {
        if (Directory.Exists(_testPath))
        {
            Directory.Delete(_testPath, true);
        }
    }
}
