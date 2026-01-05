using FidelityCard.Application.DTOs;
using FidelityCard.Application.Interfaces;
using FidelityCard.Application.UseCases;
using FidelityCard.Domain.Entities;
using FidelityCard.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace FidelityCard.Tests.UseCases;

public class RegisterFidelityUseCaseTests
{
    private readonly Mock<IFidelityRepository> _fidelityRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<ICardGeneratorService> _cardGeneratorServiceMock;
    private readonly Mock<ILogger<RegisterFidelityUseCase>> _loggerMock;
    private readonly RegisterFidelityUseCase _useCase;

    public RegisterFidelityUseCaseTests()
    {
        _fidelityRepositoryMock = new Mock<IFidelityRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _cardGeneratorServiceMock = new Mock<ICardGeneratorService>();
        _loggerMock = new Mock<ILogger<RegisterFidelityUseCase>>();

        _useCase = new RegisterFidelityUseCase(
            _fidelityRepositoryMock.Object,
            _emailServiceMock.Object,
            _cardGeneratorServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var request = CreateValidRequest();
        
        _fidelityRepositoryMock
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _fidelityRepositoryMock
            .Setup(r => r.ExistsByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _fidelityRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Fidelity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Fidelity f, CancellationToken ct) => f);

        _cardGeneratorServiceMock
            .Setup(c => c.GeneraCardDigitaleAsync(It.IsAny<FidelityDto>(), It.IsAny<string>()))
            .ReturnsAsync(new byte[] { 1, 2, 3 });

        _emailServiceMock
            .Setup(e => e.InviaEmailBenvenutoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>()))
            .ReturnsAsync(true);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("FIDELITY123", result.Data.CdFidelity);
    }

    [Fact]
    public async Task ExecuteAsync_EmailAlreadyExists_ShouldReturnFailure()
    {
        // Arrange
        var request = CreateValidRequest();
        
        _fidelityRepositoryMock
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("email", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ExecuteAsync_CodeAlreadyExists_ShouldReturnFailure()
    {
        // Arrange
        var request = CreateValidRequest();
        
        _fidelityRepositoryMock
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _fidelityRepositoryMock
            .Setup(r => r.ExistsByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("codice", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ExecuteAsync_InvalidEmail_ShouldReturnFailure()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Email = "invalid-email";
        
        _fidelityRepositoryMock
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _fidelityRepositoryMock
            .Setup(r => r.ExistsByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.False(result.Success);
    }

    private static RegisterFidelityRequest CreateValidRequest()
    {
        return new RegisterFidelityRequest
        {
            CdFidelity = "FIDELITY123",
            Store = "NE001",
            Cognome = "Rossi",
            Nome = "Mario",
            DataNascita = new DateTime(1990, 1, 1),
            Email = "mario.rossi@example.com"
        };
    }
}
