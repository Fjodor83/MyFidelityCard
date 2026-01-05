using FidelityCard.Application.DTOs;

namespace FidelityCard.Application.Interfaces;

/// <summary>
/// Interfaccia per il servizio di generazione card - definito nell'Application layer
/// </summary>
public interface ICardGeneratorService
{
    /// <summary>
    /// Genera la card digitale in formato immagine PNG
    /// </summary>
    Task<byte[]> GeneraCardDigitaleAsync(
        FidelityDto fidelity, 
        string? puntoVenditaNome = null);

    /// <summary>
    /// Genera solo il QR Code
    /// </summary>
    Task<byte[]> GeneraQRCodeAsync(
        string contenuto, 
        int dimensione = 200);
}
