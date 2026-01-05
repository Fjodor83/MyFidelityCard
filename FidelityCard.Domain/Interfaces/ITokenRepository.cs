namespace FidelityCard.Domain.Interfaces;

/// <summary>
/// Repository interface per Token - definito nel Domain layer
/// </summary>
public interface ITokenRepository
{
    /// <summary>
    /// Genera un nuovo token per email e store
    /// </summary>
    string GenerateToken(string email, string store);

    /// <summary>
    /// Recupera i dati associati al token
    /// </summary>
    Task<TokenData?> GetTokenDataAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida un token e restituisce i dati se valido
    /// </summary>
    Task<TokenData?> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina i token scaduti
    /// </summary>
    void CleanupExpiredTokens(TimeSpan maxAge);
}

/// <summary>
/// DTO per i dati del token
/// </summary>
public record TokenData(string Store, string Email);
