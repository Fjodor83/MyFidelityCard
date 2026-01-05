using FidelityCard.Domain.Entities;

namespace FidelityCard.Domain.Interfaces;

/// <summary>
/// Repository interface per Fidelity - definito nel Domain layer
/// </summary>
public interface IFidelityRepository
{
    /// <summary>
    /// Trova una fidelity per email
    /// </summary>
    Task<Fidelity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Trova una fidelity per codice fidelity
    /// </summary>
    Task<Fidelity?> GetByCodeAsync(string cdFidelity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Trova una fidelity per ID
    /// </summary>
    Task<Fidelity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se esiste una fidelity con la data email
    /// </summary>
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se esiste una fidelity con il dato codice
    /// </summary>
    Task<bool> ExistsByCodeAsync(string cdFidelity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aggiunge una nuova fidelity
    /// </summary>
    Task<Fidelity> AddAsync(Fidelity fidelity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aggiorna una fidelity esistente
    /// </summary>
    Task UpdateAsync(Fidelity fidelity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una fidelity
    /// </summary>
    Task DeleteAsync(Fidelity fidelity, CancellationToken cancellationToken = default);
}
