using FidelityCard.Application.UseCases;
using FidelityCard.Domain.Entities;
using FidelityCard.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace FidelityCard.Tests.UseCases;

public class GetProfileUseCaseTests
{
    private readonly Mock<IFidelityRepository> _fidelityRepositoryMock;
    private readonly Mock<ITokenRepository> _tokenRepositoryMock;
    private readonly Mock<ILogger<GetProfileUseCase>> _loggerMock;
    private readonly GetProfileUseCase _useCase;

    public GetProfileUseCaseTests()
    {
        _fidelityRepositoryMock = new Mock<IFidelityRepository>();
        _tokenRepositoryMock = new Mock<ITokenRepository>();
        _loggerMock = new Mock<ILogger<GetProfileUseCase>>();

        _useCase = new GetProfileUseCase(
            _fidelityRepositoryMock.Object,
            _tokenRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_ValidToken_ShouldReturnProfile()
    {
        // Arrange
        var tokenData = new TokenData("NE001", "mario.rossi@example.com");
        var fidelity = Fidelity.Create(
            cdFidelity: "FIDELITY123",
            store: "NE001",
            cognome: "Rossi",
            nome: "Mario",
            dataNascita: new DateTime(1990, 1, 1),
            email: "mario.rossi@example.com"
        );

        _tokenRepositoryMock
            .Setup(t => t.GetTokenDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenData);

        _fidelityRepositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fidelity);

        // Act
        var result = await _useCase.ExecuteAsync("valid-token");

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("Mario", result.Data.Nome);
        Assert.Equal("Rossi", result.Data.Cognome);
    }

    [Fact]
    public async Task ExecuteAsync_InvalidToken_ShouldReturnFailure()
    {
        // Arrange
        _tokenRepositoryMock
            .Setup(t => t.GetTokenDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TokenData?)null);

        // Act
        var result = await _useCase.ExecuteAsync("invalid-token");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("token", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ExecuteAsync_UserNotFound_ShouldReturnFailure()
    {
        // Arrange
        var tokenData = new TokenData("NE001", "nonexistent@example.com");

        _tokenRepositoryMock
            .Setup(t => t.GetTokenDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenData);

        _fidelityRepositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Fidelity?)null);

        // Act
        var result = await _useCase.ExecuteAsync("valid-token");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("utente", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ExecuteAsync_EmptyToken_ShouldReturnFailure(string? token)
    {
        // Act
        var result = await _useCase.ExecuteAsync(token!);

        // Assert
        Assert.False(result.Success);
    }
}
