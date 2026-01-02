namespace FidelityCard.Lib.Services;

public interface IEmailService
{
    Task<bool> InviaEmailBenvenutoAsync(string email, string nome, string codiceFidelity, byte[]? cardDigitale = null);
    Task<bool> InviaEmailVerificaAsync(string email, string nome, string token, string linkRegistrazione, string puntoVenditaNome);
    Task<bool> InviaEmailAccessoProfiloAsync(string email, string nome, string linkAccesso);
}
