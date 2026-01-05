namespace FidelityCard.Application.DTOs;

/// <summary>
/// DTO per la risposta Fidelity (usato per trasferire dati al client)
/// </summary>
public class FidelityDto
{
    public int IdFidelity { get; set; }
    public string CdFidelity { get; set; } = string.Empty;
    public string Store { get; set; } = string.Empty;
    public string Cognome { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public DateTime? DataNascita { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Sesso { get; set; }
    public string? Indirizzo { get; set; }
    public string? Localita { get; set; }
    public string? Cap { get; set; }
    public string? Provincia { get; set; }
    public string? Nazione { get; set; }
    public string? Cellulare { get; set; }
}
