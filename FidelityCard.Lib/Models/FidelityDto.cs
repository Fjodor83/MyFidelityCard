using System.ComponentModel.DataAnnotations;

namespace FidelityCard.Lib.Models;

/// <summary>
/// DTO Fidelity per trasferimento dati verso client Blazor
/// (Separato dall'entity domain)
/// </summary>
public class FidelityDto
{
    public int IdFidelity { get; set; }

    [Required(ErrorMessage = "Il codice fidelity è obbligatorio")]
    [StringLength(20, MinimumLength = 6)]
    public string CdFidelity { get; set; } = string.Empty;

    [Required(ErrorMessage = "Il codice negozio è obbligatorio")]
    [StringLength(6, MinimumLength = 2)]
    public string Store { get; set; } = string.Empty;

    [Required(ErrorMessage = "Il cognome è obbligatorio")]
    [StringLength(50)]
    public string Cognome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Il nome è obbligatorio")]
    [StringLength(50)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "La data di nascita è obbligatoria")]
    public DateTime? DataNascita { get; set; } = null;

    [Required(ErrorMessage = "L'email è obbligatoria")]
    [StringLength(100)]
    [EmailAddress(ErrorMessage = "Formato email non valido")]
    public string Email { get; set; } = string.Empty;

    [StringLength(1)]
    public string? Sesso { get; set; } = null;

    [StringLength(100)]
    public string? Indirizzo { get; set; } = null;

    [StringLength(100)]
    public string? Localita { get; set; } = null;

    [StringLength(10)]
    public string? Cap { get; set; } = null;

    [StringLength(2)]
    public string? Provincia { get; set; } = null;

    [StringLength(2)]
    public string? Nazione { get; set; } = null;

    [StringLength(20)]
    [Phone(ErrorMessage = "Formato telefono non valido")]
    public string? Cellulare { get; set; } = null;
}
