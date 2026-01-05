using FidelityCard.Application.DTOs;
using FidelityCard.Application.Mappers;
using FidelityCard.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FidelityCard.Application.UseCases;

/// <summary>
/// Use Case per il recupero del profilo utente tramite token
/// </summary>
public class GetProfileUseCase
{
    private readonly IFidelityRepository _fidelityRepository;
    private readonly ITokenRepository _tokenRepository;
    private readonly ILogger<GetProfileUseCase> _logger;

    public GetProfileUseCase(
        IFidelityRepository fidelityRepository,
        ITokenRepository tokenRepository,
        ILogger<GetProfileUseCase> logger)
    {
        _fidelityRepository = fidelityRepository;
        _tokenRepository = tokenRepository;
        _logger = logger;
    }

    public async Task<Result<FidelityDto>> ExecuteAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validazione token
            if (string.IsNullOrWhiteSpace(token))
            {
                return Result<FidelityDto>.Fail("Token non valido");
            }

            // Recupera dati dal token
            var tokenData = await _tokenRepository.GetTokenDataAsync(token, cancellationToken);

            if (tokenData == null)
            {
                return Result<FidelityDto>.Fail("Token non valido o scaduto");
            }

            // Recupera utente per email
            var user = await _fidelityRepository.GetByEmailAsync(tokenData.Email, cancellationToken);

            if (user == null)
            {
                return Result<FidelityDto>.Fail("Utente non trovato");
            }

            var dto = FidelityMapper.ToDto(user);
            return Result<FidelityDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante recupero profilo con token {Token}", token);
            return Result<FidelityDto>.Fail($"Errore durante il recupero del profilo: {ex.Message}");
        }
    }
}
