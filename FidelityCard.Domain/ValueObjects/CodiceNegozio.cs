using System.Text.RegularExpressions;

namespace FidelityCard.Domain.ValueObjects;

/// <summary>
/// Value Object per Codice Negozio (Store Code) con validazione
/// Formato: 2-6 caratteri alfanumerici (es. NE001, NE002)
/// </summary>
public sealed class CodiceNegozio : IEquatable<CodiceNegozio>
{
    private static readonly Regex CodeRegex = new(
        @"^[A-Z0-9]{2,6}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    public CodiceNegozio(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Il codice negozio è obbligatorio", nameof(value));

        value = value.Trim().ToUpperInvariant();

        if (!CodeRegex.IsMatch(value))
            throw new ArgumentException(
                "Il codice negozio deve essere di 2-6 caratteri alfanumerici", 
                nameof(value));

        Value = value;
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj) => Equals(obj as CodiceNegozio);

    public bool Equals(CodiceNegozio? other) => other is not null && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(CodiceNegozio code) => code.Value;

    public static bool operator ==(CodiceNegozio? left, CodiceNegozio? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(CodiceNegozio? left, CodiceNegozio? right) => !(left == right);
}
