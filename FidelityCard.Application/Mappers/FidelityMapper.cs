using FidelityCard.Application.DTOs;
using FidelityCard.Domain.Entities;

namespace FidelityCard.Application.Mappers;

/// <summary>
/// Mapper per conversione tra Entity e DTO
/// </summary>
public static class FidelityMapper
{
    /// <summary>
    /// Converte una Fidelity entity in FidelityDto
    /// </summary>
    public static FidelityDto ToDto(Fidelity entity)
    {
        return new FidelityDto
        {
            IdFidelity = entity.IdFidelity,
            CdFidelity = entity.CdFidelity.Value,
            Store = entity.Store.Value,
            Cognome = entity.Cognome,
            Nome = entity.Nome,
            DataNascita = entity.DataNascita,
            Email = entity.Email.Value,
            Sesso = entity.Sesso,
            Indirizzo = entity.Indirizzo,
            Localita = entity.Localita,
            Cap = entity.Cap,
            Provincia = entity.Provincia,
            Nazione = entity.Nazione,
            Cellulare = entity.Cellulare
        };
    }

    /// <summary>
    /// Converte una RegisterFidelityRequest in Fidelity entity
    /// </summary>
    public static Fidelity ToEntity(RegisterFidelityRequest request)
    {
        return Fidelity.Create(
            cdFidelity: request.CdFidelity,
            store: request.Store,
            cognome: request.Cognome,
            nome: request.Nome,
            dataNascita: request.DataNascita,
            email: request.Email,
            sesso: request.Sesso,
            indirizzo: request.Indirizzo,
            localita: request.Localita,
            cap: request.Cap,
            provincia: request.Provincia,
            nazione: request.Nazione,
            cellulare: request.Cellulare
        );
    }
}
