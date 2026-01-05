using FidelityCard.Application.DTOs;
using FidelityCard.Application.Interfaces;
using FidelityCard.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FidelityCard.Application.UseCases;

/// <summary>
/// Use Case per la validazione email e invio link di registrazione/accesso
/// </summary>
public class ValidateEmailUseCase
{
    private readonly IFidelityRepository _fidelityRepository;
    private readonly ITokenRepository _tokenRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<ValidateEmailUseCase> _logger;

    public ValidateEmailUseCase(
        IFidelityRepository fidelityRepository,
        ITokenRepository tokenRepository,
        IEmailService emailService,
        ILogger<ValidateEmailUseCase> logger)
    {
        _fidelityRepository = fidelityRepository;
        _tokenRepository = tokenRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Result<ValidateEmailResponse>> ExecuteAsync(
        string email,
        string? store,
        string baseUrl,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validazione email format
            if (string.IsNullOrWhiteSpace(email))
            {
                return Result<ValidateEmailResponse>.Fail("L'email è obbligatoria");
            }

            // Normalizza store
            var storeCode = store ?? "NE001";

            // Genera token
            var token = _tokenRepository.GenerateToken(email, storeCode);

            // Verifica se l'utente esiste già
            var existingUser = await _fidelityRepository.GetByEmailAsync(email, cancellationToken);

            if (existingUser != null)
            {
                // UTENTE ESISTENTE: Invio link per ACCEDERE AL PROFILO
                var profileUrl = $"{baseUrl}/profilo?token={token}";
                await _emailService.InviaEmailAccessoProfiloAsync(
                    email, 
                    existingUser.Nome, 
                    profileUrl);

                return Result<ValidateEmailResponse>.Ok(new ValidateEmailResponse
                {
                    UserExists = true,
                    Message = "Email di accesso inviata. Controlla la tua casella di posta."
                });
            }
            else
            {
                // NUOVO UTENTE: Invio link per COMPLETARE REGISTRAZIONE
                var registrationUrl = $"{baseUrl}/Fidelity-form?token={token}";
                await _emailService.InviaEmailVerificaAsync(
                    email, 
                    "Cliente", 
                    token, 
                    registrationUrl, 
                    storeCode);

                return Result<ValidateEmailResponse>.Ok(new ValidateEmailResponse
                {
                    UserExists = false,
                    Message = "Email di verifica inviata. Controlla la tua casella di posta."
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante validazione email {Email}", email);
            return Result<ValidateEmailResponse>.Fail($"Errore durante la validazione: {ex.Message}");
        }
    }
}
