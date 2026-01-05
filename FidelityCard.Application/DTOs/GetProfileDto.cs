namespace FidelityCard.Application.DTOs;

/// <summary>
/// DTO per la richiesta di profilo
/// </summary>
public class GetProfileRequest
{
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// DTO per la risposta del profilo
/// </summary>
public class GetProfileResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public FidelityDto? Profile { get; set; }
}
