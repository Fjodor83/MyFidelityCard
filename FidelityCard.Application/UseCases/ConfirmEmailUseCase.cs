using FidelityCard.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FidelityCard.Application.UseCases;

/// <summary>
/// Use Case per conferma email (recupero dati token)
/// </summary>
public class ConfirmEmailUseCase
{
    private readonly ITokenRepository _tokenRepository;
    private readonly ILogger<ConfirmEmailUseCase> _logger;

    public ConfirmEmailUseCase(
        ITokenRepository tokenRepository,
        ILogger<ConfirmEmailUseCase> logger)
    {
        _tokenRepository = tokenRepository;
        _logger = logger;
    }

    /// <summary>
    /// Conferma il token e restituisce i dati associati (store\r\nemail)
    /// </summary>
    public async Task<string> ExecuteAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return string.Empty;
            }

            var tokenData = await _tokenRepository.GetTokenDataAsync(token, cancellationToken);

            if (tokenData == null)
            {
                return string.Empty;
            }

            // Formato legacy: store\r\nemail
            return $"{tokenData.Store}\r\n{tokenData.Email}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante conferma email con token {Token}", token);
            return string.Empty;
        }
    }
}
