using FidelityCard.Application.DTOs;

namespace FidelityCard.Application.Interfaces;

/// <summary>
/// Interfaccia per il servizio di invio email - definito nell'Application layer
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Invia email di benvenuto con card digitale allegata
    /// </summary>
    Task<bool> InviaEmailBenvenutoAsync(
        string email, 
        string nome, 
        string codiceFidelity, 
        byte[]? cardDigitale = null);

    /// <summary>
    /// Invia email di verifica per nuova registrazione
    /// </summary>
    Task<bool> InviaEmailVerificaAsync(
        string email, 
        string nome, 
        string token, 
        string linkRegistrazione, 
        string? puntoVenditaNome);

    /// <summary>
    /// Invia email con link per accesso al profilo (utente esistente)
    /// </summary>
    Task<bool> InviaEmailAccessoProfiloAsync(
        string email, 
        string nome, 
        string linkAccesso);
}
