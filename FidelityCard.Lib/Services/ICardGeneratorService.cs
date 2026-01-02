using FidelityCard.Lib.Models;

namespace FidelityCard.Lib.Services;

public interface ICardGeneratorService
{
    Task<byte[]> GeneraCardDigitaleAsync(Fidelity fidelity, string? puntoVenditaNome = null);
    Task<byte[]> GeneraQRCodeAsync(string contenuto, int dimensione = 200);
}
