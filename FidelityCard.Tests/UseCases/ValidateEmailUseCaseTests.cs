using FidelityCard.Application.DTOs;
using FidelityCard.Application.Interfaces;
using FidelityCard.Application.UseCases;
using FidelityCard.Domain.Entities;
using FidelityCard.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace FidelityCard.Tests.UseCases;

public class ValidateEmailUseCaseTests
{
    private readonly Mock<IFidelityRepository> _fidelityRepositoryMock;
    private readonly Mock<ITokenRepository> _tokenRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<ILogger<ValidateEmailUseCase>> _loggerMock;
    private readonly ValidateEmailUseCase _useCase;

    public ValidateEmailUseCaseTests()
    {
        _fidelityRepositoryMock = new Mock<IFidelityRepository>();
        _tokenRepositoryMock = new Mock<ITokenRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _loggerMock = new Mock<ILogger<ValidateEmailUseCase>>();

        _useCase = new ValidateEmailUseCase(
            _fidelityRepositoryMock.Object,
            _tokenRepositoryMock.Object,
            _emailServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_ExistingUser_ShouldReturnUserExists()
    {
        // Arrange
        var existingFidelity = Fidelity.Create(
            cdFidelity: "FIDELITY123",
            store: "NE001",
            cognome: "Rossi",
            nome: "Mario",
            dataNascita: new DateTime(1990, 1, 1),
            email: "mario.rossi@example.com"
        );

        _tokenRepositoryMock
            .Setup(t => t.GenerateToken(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("test-token");

        _fidelityRepositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingFidelity);

        _emailServiceMock
            .Setup(e => e.InviaEmailAccessoProfiloAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var result = await _useCase.ExecuteAsync("mario.rossi@example.com", "NE001", "https://test.com");

        // Assert
        Assert.True(result.Success);
        Assert.True(result.Data!.UserExists);
        _emailServiceMock.Verify(e => e.InviaEmailAccessoProfiloAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_NewUser_ShouldReturnUserNotExists()
    {
        // Arrange
        _tokenRepositoryMock
            .Setup(t => t.GenerateToken(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("test-token");

        _fidelityRepositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Fidelity?)null);

        _emailServiceMock
            .Setup(e => e.InviaEmailVerificaAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>()))
            .ReturnsAsync(true);

        // Act
        var result = await _useCase.ExecuteAsync("new.user@example.com", "NE001", "https://test.com");

        // Assert
        Assert.True(result.Success);
        Assert.False(result.Data!.UserExists);
        _emailServiceMock.Verify(e => e.InviaEmailVerificaAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyEmail_ShouldReturnFailure()
    {
        // Act
        var result = await _useCase.ExecuteAsync("", "NE001", "https://test.com");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("email", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ExecuteAsync_NullStore_ShouldUseDefaultStore()
    {
        // Arrange
        _tokenRepositoryMock
            .Setup(t => t.GenerateToken(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("test-token");

        _fidelityRepositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Fidelity?)null);

        _emailServiceMock
            .Setup(e => e.InviaEmailVerificaAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>()))
            .ReturnsAsync(true);

        // Act
        var result = await _useCase.ExecuteAsync("test@example.com", null, "https://test.com");

        // Assert
        Assert.True(result.Success);
        _tokenRepositoryMock.Verify(t => t.GenerateToken(It.IsAny<string>(), "NE001"), Times.Once);
    }
}
