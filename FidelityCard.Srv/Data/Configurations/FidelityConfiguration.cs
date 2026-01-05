using FidelityCard.Domain.Entities;
using FidelityCard.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FidelityCard.Srv.Data.Configurations;

/// <summary>
/// Fluent API configuration per l'entity Fidelity
/// </summary>
public class FidelityConfiguration : IEntityTypeConfiguration<Fidelity>
{
    public void Configure(EntityTypeBuilder<Fidelity> builder)
    {
        builder.ToTable("Fidelity");

        // Primary Key
        builder.HasKey(f => f.IdFidelity);
        builder.Property(f => f.IdFidelity)
            .ValueGeneratedOnAdd();

        // CdFidelity - Value Object conversion con ValueComparer
        builder.Property(f => f.CdFidelity)
            .HasColumnName("CdFidelity")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => new CodiceFidelity(v))
            .Metadata.SetValueComparer(new ValueComparer<CodiceFidelity>(
                (c1, c2) => c1 != null && c2 != null && c1.Value == c2.Value,
                c => c.Value.GetHashCode(),
                c => new CodiceFidelity(c.Value)));

        builder.HasIndex(f => f.CdFidelity)
            .IsUnique();

        // Store (CdNe) - Value Object conversion con ValueComparer
        builder.Property(f => f.Store)
            .HasColumnName("CdNe")
            .HasColumnType("varchar(6)")
            .HasMaxLength(6)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => new CodiceNegozio(v))
            .Metadata.SetValueComparer(new ValueComparer<CodiceNegozio>(
                (c1, c2) => c1 != null && c2 != null && c1.Value == c2.Value,
                c => c.Value.GetHashCode(),
                c => new CodiceNegozio(c.Value)));

        // Cognome
        builder.Property(f => f.Cognome)
            .HasColumnType("varchar(50)")
            .HasMaxLength(50)
            .IsRequired();

        // Nome
        builder.Property(f => f.Nome)
            .HasColumnType("varchar(50)")
            .HasMaxLength(50)
            .IsRequired();

        // DataNascita
        builder.Property(f => f.DataNascita)
            .HasColumnType("smalldatetime")
            .IsRequired();

        // Email - Value Object conversion con ValueComparer
        builder.Property(f => f.Email)
            .HasColumnName("Email")
            .HasColumnType("varchar(100)")
            .HasMaxLength(100)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => new Email(v))
            .Metadata.SetValueComparer(new ValueComparer<Email>(
                (e1, e2) => e1 != null && e2 != null && e1.Value == e2.Value,
                e => e.Value.GetHashCode(),
                e => new Email(e.Value)));

        builder.HasIndex(f => f.Email)
            .IsUnique();

        // Sesso
        builder.Property(f => f.Sesso)
            .HasColumnType("char(1)")
            .HasMaxLength(1);

        // Indirizzo
        builder.Property(f => f.Indirizzo)
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);

        // Localita
        builder.Property(f => f.Localita)
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);

        // Cap
        builder.Property(f => f.Cap)
            .HasColumnType("varchar(10)")
            .HasMaxLength(10);

        // Provincia
        builder.Property(f => f.Provincia)
            .HasColumnType("char(2)")
            .HasMaxLength(2);

        // Nazione
        builder.Property(f => f.Nazione)
            .HasColumnType("char(2)")
            .HasMaxLength(2);

        // Cellulare
        builder.Property(f => f.Cellulare)
            .HasColumnType("varchar(20)")
            .HasMaxLength(20);
    }
}
