using FidelityCard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
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

        // CdFidelity - Value Object conversion
        builder.Property(f => f.CdFidelity)
            .HasColumnName("CdFidelity")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => new Domain.ValueObjects.CodiceFidelity(v));

        builder.HasIndex(f => f.CdFidelity)
            .IsUnique();

        // Store (CdNe) - Value Object conversion
        builder.Property(f => f.Store)
            .HasColumnName("CdNe")
            .HasColumnType("varchar(6)")
            .HasMaxLength(6)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => new Domain.ValueObjects.CodiceNegozio(v));

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

        // Email - Value Object conversion
        builder.Property(f => f.Email)
            .HasColumnName("Email")
            .HasColumnType("varchar(100)")
            .HasMaxLength(100)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => new Domain.ValueObjects.Email(v));

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
