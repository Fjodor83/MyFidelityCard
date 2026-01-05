using FidelityCard.Domain.ValueObjects;

namespace FidelityCard.Tests.ValueObjects;

public class EmailValueObjectTests
{
    [Fact]
    public void Email_ValidEmail_ShouldCreate()
    {
        // Arrange & Act
        var email = new Email("test@example.com");

        // Assert
        Assert.Equal("test@example.com", email.Value);
    }

    [Fact]
    public void Email_ShouldNormalize_ToLowerCase()
    {
        // Arrange & Act
        var email = new Email("Test@Example.COM");

        // Assert
        Assert.Equal("test@example.com", email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Email_EmptyOrNull_ShouldThrow(string? value)
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Email(value!));
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("invalid@")]
    [InlineData("@invalid.com")]
    [InlineData("invalid@.com")]
    public void Email_InvalidFormat_ShouldThrow(string value)
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Email(value));
    }

    [Fact]
    public void Email_TooLong_ShouldThrow()
    {
        // Arrange - 92 + "@test.com" (9 chars) = 101 chars, > 100
        var longEmail = new string('a', 92) + "@test.com";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Email(longEmail));
    }

    [Fact]
    public void Email_Equality_ShouldWork()
    {
        // Arrange
        var email1 = new Email("test@example.com");
        var email2 = new Email("TEST@EXAMPLE.COM");

        // Assert
        Assert.Equal(email1, email2);
        Assert.True(email1 == email2);
    }

    [Fact]
    public void Email_ImplicitConversion_ShouldWork()
    {
        // Arrange
        var email = new Email("test@example.com");

        // Act
        string value = email;

        // Assert
        Assert.Equal("test@example.com", value);
    }
}
