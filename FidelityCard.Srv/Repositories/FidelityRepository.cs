using FidelityCard.Domain.Entities;
using FidelityCard.Domain.Interfaces;
using FidelityCard.Domain.ValueObjects;
using FidelityCard.Srv.Data;
using Microsoft.EntityFrameworkCore;

namespace FidelityCard.Srv.Repositories;

/// <summary>
/// Implementazione del repository per Fidelity
/// </summary>
public class FidelityRepository : IFidelityRepository
{
    private readonly FidelityCardDbContext _context;

    public FidelityRepository(FidelityCardDbContext context)
    {
        _context = context;
    }

    public async Task<Fidelity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        // Normalizza email come fa il Value Object
        var normalizedEmail = email.Trim().ToLowerInvariant();
        
        // Usa FromSqlRaw per evitare problemi con Value Object conversion in query
        return await _context.Fidelity
            .FromSqlRaw("SELECT * FROM Fidelity WHERE Email = {0}", normalizedEmail)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Fidelity?> GetByCodeAsync(string cdFidelity, CancellationToken cancellationToken = default)
    {
        var normalizedCode = cdFidelity.Trim().ToUpperInvariant();
        
        return await _context.Fidelity
            .FromSqlRaw("SELECT * FROM Fidelity WHERE CdFidelity = {0}", normalizedCode)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Fidelity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Fidelity.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        
        return await _context.Fidelity
            .FromSqlRaw("SELECT * FROM Fidelity WHERE Email = {0}", normalizedEmail)
            .AnyAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string cdFidelity, CancellationToken cancellationToken = default)
    {
        var normalizedCode = cdFidelity.Trim().ToUpperInvariant();
        
        return await _context.Fidelity
            .FromSqlRaw("SELECT * FROM Fidelity WHERE CdFidelity = {0}", normalizedCode)
            .AnyAsync(cancellationToken);
    }

    public async Task<Fidelity> AddAsync(Fidelity fidelity, CancellationToken cancellationToken = default)
    {
        _context.Fidelity.Add(fidelity);
        await _context.SaveChangesAsync(cancellationToken);
        return fidelity;
    }

    public async Task UpdateAsync(Fidelity fidelity, CancellationToken cancellationToken = default)
    {
        _context.Fidelity.Update(fidelity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Fidelity fidelity, CancellationToken cancellationToken = default)
    {
        _context.Fidelity.Remove(fidelity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
