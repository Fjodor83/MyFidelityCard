using FidelityCard.Application.DTOs;
using FidelityCard.Application.Mappers;
using FidelityCard.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FidelityCard.Application.UseCases;

/// <summary>
/// Use Case per il recupero di una Fidelity per email
/// </summary>
public class GetFidelityByEmailUseCase
{
    private readonly IFidelityRepository _fidelityRepository;
    private readonly ILogger<GetFidelityByEmailUseCase> _logger;

    public GetFidelityByEmailUseCase(
        IFidelityRepository fidelityRepository,
        ILogger<GetFidelityByEmailUseCase> logger)
    {
        _fidelityRepository = fidelityRepository;
        _logger = logger;
    }

    public async Task<FidelityDto> ExecuteAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new FidelityDto();
            }

            var fidelity = await _fidelityRepository.GetByEmailAsync(email, cancellationToken);

            if (fidelity == null)
            {
                return new FidelityDto();
            }

            return FidelityMapper.ToDto(fidelity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante recupero fidelity per email {Email}", email);
            return new FidelityDto();
        }
    }
}
