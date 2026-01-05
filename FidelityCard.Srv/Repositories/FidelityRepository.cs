using FidelityCard.Domain.Entities;
using FidelityCard.Domain.Interfaces;
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
        return await _context.Fidelity
            .FirstOrDefaultAsync(f => EF.Property<string>(f, "Email") == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<Fidelity?> GetByCodeAsync(string cdFidelity, CancellationToken cancellationToken = default)
    {
        return await _context.Fidelity
            .FirstOrDefaultAsync(f => EF.Property<string>(f, "CdFidelity") == cdFidelity.ToUpperInvariant(), cancellationToken);
    }

    public async Task<Fidelity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Fidelity.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Fidelity
            .AnyAsync(f => EF.Property<string>(f, "Email") == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string cdFidelity, CancellationToken cancellationToken = default)
    {
        return await _context.Fidelity
            .AnyAsync(f => EF.Property<string>(f, "CdFidelity") == cdFidelity.ToUpperInvariant(), cancellationToken);
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
