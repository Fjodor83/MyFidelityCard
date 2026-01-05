using FidelityCard.Domain.ValueObjects;

namespace FidelityCard.Tests.ValueObjects;

public class CodiceNegozioValueObjectTests
{
    [Theory]
    [InlineData("NE001")]
    [InlineData("AB")]
    [InlineData("ABCDEF")]
    public void CodiceNegozio_ValidCode_ShouldCreate(string code)
    {
        // Arrange & Act
        var codiceNegozio = new CodiceNegozio(code);

        // Assert
        Assert.Equal(code.ToUpperInvariant(), codiceNegozio.Value);
    }

    [Fact]
    public void CodiceNegozio_ShouldNormalize_ToUpperCase()
    {
        // Arrange & Act
        var codice = new CodiceNegozio("ne001");

        // Assert
        Assert.Equal("NE001", codice.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CodiceNegozio_EmptyOrNull_ShouldThrow(string? value)
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new CodiceNegozio(value!));
    }

    [Theory]
    [InlineData("A")] // Too short
    [InlineData("ABCDEFG")] // Too long (> 6)
    [InlineData("AB-01")] // Invalid characters
    public void CodiceNegozio_InvalidFormat_ShouldThrow(string value)
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new CodiceNegozio(value));
    }

    [Fact]
    public void CodiceNegozio_Equality_ShouldWork()
    {
        // Arrange
        var code1 = new CodiceNegozio("NE001");
        var code2 = new CodiceNegozio("ne001");

        // Assert
        Assert.Equal(code1, code2);
        Assert.True(code1 == code2);
    }
}
