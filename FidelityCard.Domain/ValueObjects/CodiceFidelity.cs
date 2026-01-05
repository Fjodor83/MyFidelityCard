using System.Text.RegularExpressions;

namespace FidelityCard.Domain.ValueObjects;

/// <summary>
/// Value Object per Codice Fidelity Card con validazione
/// Formato: 6-20 caratteri alfanumerici
/// </summary>
public sealed class CodiceFidelity : IEquatable<CodiceFidelity>
{
    private static readonly Regex CodeRegex = new(
        @"^[A-Z0-9]{6,20}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    public CodiceFidelity(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Il codice fidelity è obbligatorio", nameof(value));

        value = value.Trim().ToUpperInvariant();

        if (!CodeRegex.IsMatch(value))
            throw new ArgumentException(
                "Il codice fidelity deve essere di 6-20 caratteri alfanumerici", 
                nameof(value));

        Value = value;
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj) => Equals(obj as CodiceFidelity);

    public bool Equals(CodiceFidelity? other) => other is not null && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(CodiceFidelity code) => code.Value;

    public static bool operator ==(CodiceFidelity? left, CodiceFidelity? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(CodiceFidelity? left, CodiceFidelity? right) => !(left == right);
}
