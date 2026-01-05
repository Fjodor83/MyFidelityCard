using FidelityCard.Domain.Entities;

namespace FidelityCard.Tests.Entities;

public class FidelityEntityTests
{
    [Fact]
    public void Fidelity_Create_WithValidData_ShouldSucceed()
    {
        // Arrange & Act
        var fidelity = Fidelity.Create(
            cdFidelity: "FIDELITY123",
            store: "NE001",
            cognome: "Rossi",
            nome: "Mario",
            dataNascita: new DateTime(1990, 1, 1),
            email: "mario.rossi@example.com"
        );

        // Assert
        Assert.NotNull(fidelity);
        Assert.Equal("FIDELITY123", fidelity.CdFidelity.Value);
        Assert.Equal("NE001", fidelity.Store.Value);
        Assert.Equal("Rossi", fidelity.Cognome);
        Assert.Equal("Mario", fidelity.Nome);
        Assert.Equal("mario.rossi@example.com", fidelity.Email.Value);
    }

    [Fact]
    public void Fidelity_Create_WithOptionalFields_ShouldSucceed()
    {
        // Arrange & Act
        var fidelity = Fidelity.Create(
            cdFidelity: "FIDELITY123",
            store: "NE001",
            cognome: "Rossi",
            nome: "Mario",
            dataNascita: new DateTime(1990, 1, 1),
            email: "mario.rossi@example.com",
            sesso: "M",
            indirizzo: "Via Roma 1",
            localita: "Milano",
            cap: "20100",
            provincia: "MI",
            nazione: "IT",
            cellulare: "+39 123 456 7890"
        );

        // Assert
        Assert.Equal("M", fidelity.Sesso);
        Assert.Equal("Via Roma 1", fidelity.Indirizzo);
        Assert.Equal("Milano", fidelity.Localita);
        Assert.Equal("20100", fidelity.Cap);
        Assert.Equal("MI", fidelity.Provincia);
        Assert.Equal("IT", fidelity.Nazione);
        Assert.Equal("+39 123 456 7890", fidelity.Cellulare);
    }

    [Fact]
    public void Fidelity_Create_WithInvalidEmail_ShouldThrow()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Fidelity.Create(
            cdFidelity: "FIDELITY123",
            store: "NE001",
            cognome: "Rossi",
            nome: "Mario",
            dataNascita: new DateTime(1990, 1, 1),
            email: "invalid-email"
        ));
    }

    [Fact]
    public void Fidelity_Create_WithTooRecentBirthDate_ShouldThrow()
    {
        // Arrange - Data di nascita di 1 anno fa (troppo recente, deve essere almeno 6 anni)
        var recentDate = DateTime.Today.AddYears(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Fidelity.Create(
            cdFidelity: "FIDELITY123",
            store: "NE001",
            cognome: "Rossi",
            nome: "Mario",
            dataNascita: recentDate,
            email: "mario.rossi@example.com"
        ));
    }

    [Fact]
    public void Fidelity_Create_WithTooOldBirthDate_ShouldThrow()
    {
        // Arrange - Data di nascita di 110 anni fa (troppo vecchia, max 100 anni)
        var oldDate = DateTime.Today.AddYears(-110);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Fidelity.Create(
            cdFidelity: "FIDELITY123",
            store: "NE001",
            cognome: "Rossi",
            nome: "Mario",
            dataNascita: oldDate,
            email: "mario.rossi@example.com"
        ));
    }

    [Fact]
    public void Fidelity_Create_WithEmptyNome_ShouldThrow()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Fidelity.Create(
            cdFidelity: "FIDELITY123",
            store: "NE001",
            cognome: "Rossi",
            nome: "",
            dataNascita: new DateTime(1990, 1, 1),
            email: "mario.rossi@example.com"
        ));
    }

    [Fact]
    public void Fidelity_Create_WithInvalidPhone_ShouldThrow()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Fidelity.Create(
            cdFidelity: "FIDELITY123",
            store: "NE001",
            cognome: "Rossi",
            nome: "Mario",
            dataNascita: new DateTime(1990, 1, 1),
            email: "mario.rossi@example.com",
            cellulare: "invalid!phone@number"
        ));
    }

    [Fact]
    public void Fidelity_Reconstitute_ShouldRestoreAllFields()
    {
        // Arrange & Act
        var fidelity = Fidelity.Reconstitute(
            idFidelity: 1,
            cdFidelity: "FIDELITY123",
            store: "NE001",
            cognome: "Rossi",
            nome: "Mario",
            dataNascita: new DateTime(1990, 1, 1),
            email: "mario.rossi@example.com",
            sesso: "M",
            indirizzo: "Via Roma 1",
            localita: "Milano",
            cap: "20100",
            provincia: "MI",
            nazione: "IT",
            cellulare: "+39 123 456 7890"
        );

        // Assert
        Assert.Equal(1, fidelity.IdFidelity);
        Assert.Equal("FIDELITY123", fidelity.CdFidelity.Value);
        Assert.Equal("NE001", fidelity.Store.Value);
    }
}
