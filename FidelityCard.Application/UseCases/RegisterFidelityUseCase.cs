using FidelityCard.Application.DTOs;
using FidelityCard.Application.Interfaces;
using FidelityCard.Application.Mappers;
using FidelityCard.Domain.Entities;
using FidelityCard.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FidelityCard.Application.UseCases;

/// <summary>
/// Use Case per la registrazione di una nuova Fidelity Card
/// </summary>
public class RegisterFidelityUseCase
{
    private readonly IFidelityRepository _fidelityRepository;
    private readonly IEmailService _emailService;
    private readonly ICardGeneratorService _cardGeneratorService;
    private readonly ILogger<RegisterFidelityUseCase> _logger;

    public RegisterFidelityUseCase(
        IFidelityRepository fidelityRepository,
        IEmailService emailService,
        ICardGeneratorService cardGeneratorService,
        ILogger<RegisterFidelityUseCase> logger)
    {
        _fidelityRepository = fidelityRepository;
        _emailService = emailService;
        _cardGeneratorService = cardGeneratorService;
        _logger = logger;
    }

    public async Task<Result<FidelityDto>> ExecuteAsync(
        RegisterFidelityRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validazione business: verifica email non già registrata
            if (await _fidelityRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            {
                return Result<FidelityDto>.Fail("Esiste già una fidelity card associata a questa email");
            }

            // Validazione business: verifica codice fidelity non già usato
            if (await _fidelityRepository.ExistsByCodeAsync(request.CdFidelity, cancellationToken))
            {
                return Result<FidelityDto>.Fail("Il codice fidelity è già in uso");
            }

            // Creazione entity tramite factory method (valida i Value Objects)
            Fidelity fidelity;
            try
            {
                fidelity = FidelityMapper.ToEntity(request);
            }
            catch (ArgumentException ex)
            {
                return Result<FidelityDto>.Fail(ex.Message);
            }

            // Salvataggio nel repository
            var savedFidelity = await _fidelityRepository.AddAsync(fidelity, cancellationToken);
            var dto = FidelityMapper.ToDto(savedFidelity);

            // Generazione card e invio email (non bloccante)
            try
            {
                var cardBytes = await _cardGeneratorService.GeneraCardDigitaleAsync(dto, dto.Store);
                await _emailService.InviaEmailBenvenutoAsync(
                    dto.Email, 
                    dto.Nome, 
                    dto.CdFidelity, 
                    cardBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante generazione card o invio email benvenuto per {Email}", dto.Email);
                // Non blocchiamo il ritorno OK, la registrazione è avvenuta
            }

            return Result<FidelityDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante registrazione fidelity per email {Email}", request.Email);
            return Result<FidelityDto>.Fail($"Errore durante la registrazione: {ex.Message}");
        }
    }
}
