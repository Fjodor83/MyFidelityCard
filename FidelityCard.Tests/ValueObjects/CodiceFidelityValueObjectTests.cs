using FidelityCard.Domain.ValueObjects;

namespace FidelityCard.Tests.ValueObjects;

public class CodiceFidelityValueObjectTests
{
    [Theory]
    [InlineData("ABC123")]
    [InlineData("FIDELITY123456789")]
    [InlineData("12345678901234567890")] // 20 chars - max
    public void CodiceFidelity_ValidCode_ShouldCreate(string code)
    {
        // Arrange & Act
        var codiceFidelity = new CodiceFidelity(code);

        // Assert
        Assert.Equal(code.ToUpperInvariant(), codiceFidelity.Value);
    }

    [Fact]
    public void CodiceFidelity_ShouldNormalize_ToUpperCase()
    {
        // Arrange & Act
        var codice = new CodiceFidelity("abc123def456");

        // Assert
        Assert.Equal("ABC123DEF456", codice.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CodiceFidelity_EmptyOrNull_ShouldThrow(string? value)
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new CodiceFidelity(value!));
    }

    [Theory]
    [InlineData("ABC12")] // Too short (< 6)
    [InlineData("123456789012345678901")] // Too long (> 20)
    [InlineData("ABC-123")] // Invalid characters
    public void CodiceFidelity_InvalidFormat_ShouldThrow(string value)
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new CodiceFidelity(value));
    }

    [Fact]
    public void CodiceFidelity_Equality_ShouldWork()
    {
        // Arrange
        var code1 = new CodiceFidelity("ABC123DEF");
        var code2 = new CodiceFidelity("abc123def");

        // Assert
        Assert.Equal(code1, code2);
        Assert.True(code1 == code2);
    }

    [Fact]
    public void CodiceFidelity_ImplicitConversion_ShouldWork()
    {
        // Arrange
        var codice = new CodiceFidelity("FIDELITY123");

        // Act
        string value = codice;

        // Assert
        Assert.Equal("FIDELITY123", value);
    }
}
