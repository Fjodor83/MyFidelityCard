using System.ComponentModel.DataAnnotations;

namespace FidelityCard.Application.DTOs;

/// <summary>
/// DTO per la richiesta di registrazione Fidelity
/// </summary>
public class RegisterFidelityRequest
{
    [Required(ErrorMessage = "Il codice fidelity è obbligatorio")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "Il codice fidelity deve essere tra 6 e 20 caratteri")]
    public string CdFidelity { get; set; } = string.Empty;

    [Required(ErrorMessage = "Il codice negozio è obbligatorio")]
    [StringLength(6, MinimumLength = 2, ErrorMessage = "Il codice negozio deve essere tra 2 e 6 caratteri")]
    public string Store { get; set; } = string.Empty;

    [Required(ErrorMessage = "Il cognome è obbligatorio")]
    [StringLength(50, ErrorMessage = "Il cognome non può superare 50 caratteri")]
    public string Cognome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Il nome è obbligatorio")]
    [StringLength(50, ErrorMessage = "Il nome non può superare 50 caratteri")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "La data di nascita è obbligatoria")]
    public DateTime? DataNascita { get; set; }

    [Required(ErrorMessage = "L'email è obbligatoria")]
    [EmailAddress(ErrorMessage = "Formato email non valido")]
    [StringLength(100, ErrorMessage = "L'email non può superare 100 caratteri")]
    public string Email { get; set; } = string.Empty;

    [StringLength(1)]
    public string? Sesso { get; set; }

    [StringLength(100)]
    public string? Indirizzo { get; set; }

    [StringLength(100)]
    public string? Localita { get; set; }

    [StringLength(10)]
    public string? Cap { get; set; }

    [StringLength(2)]
    public string? Provincia { get; set; }

    [StringLength(2)]
    public string? Nazione { get; set; }

    [Phone(ErrorMessage = "Formato numero di telefono non valido")]
    [StringLength(20)]
    public string? Cellulare { get; set; }
}
