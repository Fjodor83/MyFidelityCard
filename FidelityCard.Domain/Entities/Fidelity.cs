using FidelityCard.Domain.ValueObjects;

namespace FidelityCard.Domain.Entities;

/// <summary>
/// Domain Entity per la Fidelity Card - senza dipendenze da infrastructure
/// </summary>
public class Fidelity
{
    public int IdFidelity { get; private set; }
    public CodiceFidelity CdFidelity { get; private set; } = null!;
    public CodiceNegozio Store { get; private set; } = null!;
    public string Cognome { get; private set; } = string.Empty;
    public string Nome { get; private set; } = string.Empty;
    public DateTime? DataNascita { get; private set; }
    public Email Email { get; private set; } = null!;
    public string? Sesso { get; private set; }
    public string? Indirizzo { get; private set; }
    public string? Localita { get; private set; }
    public string? Cap { get; private set; }
    public string? Provincia { get; private set; }
    public string? Nazione { get; private set; }
    public string? Cellulare { get; private set; }

    // Costruttore privato per EF Core
    private Fidelity() { }

    // Factory method per creare una nuova Fidelity
    public static Fidelity Create(
        string cdFidelity,
        string store,
        string cognome,
        string nome,
        DateTime? dataNascita,
        string email,
        string? sesso = null,
        string? indirizzo = null,
        string? localita = null,
        string? cap = null,
        string? provincia = null,
        string? nazione = null,
        string? cellulare = null)
    {
        // Validazione data nascita
        if (dataNascita.HasValue)
        {
            var minDate = DateTime.Today.AddYears(-100);
            var maxDate = DateTime.Today.AddYears(-6);
            if (dataNascita.Value < minDate || dataNascita.Value > maxDate)
            {
                throw new ArgumentException($"La data di nascita deve essere compresa tra {minDate:dd/MM/yyyy} e {maxDate:dd/MM/yyyy}");
            }
        }

        // Validazione cellulare
        if (!string.IsNullOrEmpty(cellulare) && !IsValidPhone(cellulare))
        {
            throw new ArgumentException("Formato numero di telefono non valido");
        }

        return new Fidelity
        {
            CdFidelity = new CodiceFidelity(cdFidelity),
            Store = new CodiceNegozio(store),
            Cognome = ValidateRequiredString(cognome, nameof(cognome), 50),
            Nome = ValidateRequiredString(nome, nameof(nome), 50),
            DataNascita = dataNascita,
            Email = new Email(email),
            Sesso = ValidateOptionalString(sesso, 1),
            Indirizzo = ValidateOptionalString(indirizzo, 100),
            Localita = ValidateOptionalString(localita, 100),
            Cap = ValidateOptionalString(cap, 10),
            Provincia = ValidateOptionalString(provincia, 2),
            Nazione = ValidateOptionalString(nazione, 2),
            Cellulare = ValidateOptionalString(cellulare, 20)
        };
    }

    // Per ricostruzione da database
    public static Fidelity Reconstitute(
        int idFidelity,
        string cdFidelity,
        string store,
        string cognome,
        string nome,
        DateTime? dataNascita,
        string email,
        string? sesso,
        string? indirizzo,
        string? localita,
        string? cap,
        string? provincia,
        string? nazione,
        string? cellulare)
    {
        return new Fidelity
        {
            IdFidelity = idFidelity,
            CdFidelity = new CodiceFidelity(cdFidelity),
            Store = new CodiceNegozio(store),
            Cognome = cognome,
            Nome = nome,
            DataNascita = dataNascita,
            Email = new Email(email),
            Sesso = sesso,
            Indirizzo = indirizzo,
            Localita = localita,
            Cap = cap,
            Provincia = provincia,
            Nazione = nazione,
            Cellulare = cellulare
        };
    }

    private static string ValidateRequiredString(string value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} è obbligatorio");
        
        if (value.Length > maxLength)
            throw new ArgumentException($"{fieldName} non può superare {maxLength} caratteri");
        
        return value.Trim();
    }

    private static string? ValidateOptionalString(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        
        if (value.Length > maxLength)
            throw new ArgumentException($"Il valore non può superare {maxLength} caratteri");
        
        return value.Trim();
    }

    private static bool IsValidPhone(string phone)
    {
        // Semplice validazione: solo numeri, spazi, +, -
        return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[\d\s\+\-\.\(\)]+$");
    }
}
