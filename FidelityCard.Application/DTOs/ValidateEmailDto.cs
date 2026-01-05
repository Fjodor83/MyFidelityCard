namespace FidelityCard.Application.DTOs;

/// <summary>
/// DTO per la richiesta di validazione email
/// </summary>
public class ValidateEmailRequest
{
    public string Email { get; set; } = string.Empty;
    public string? Store { get; set; }
}

/// <summary>
/// DTO per la risposta di validazione email
/// </summary>
public class ValidateEmailResponse
{
    public bool UserExists { get; set; }
    public string Message { get; set; } = string.Empty;
}
