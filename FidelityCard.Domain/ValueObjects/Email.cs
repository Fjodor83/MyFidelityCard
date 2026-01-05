using System.Text.RegularExpressions;

namespace FidelityCard.Domain.ValueObjects;

/// <summary>
/// Value Object per Email con validazione
/// </summary>
public sealed class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("L'email è obbligatoria", nameof(value));

        value = value.Trim().ToLowerInvariant();

        if (value.Length > 100)
            throw new ArgumentException("L'email non può superare 100 caratteri", nameof(value));

        if (!EmailRegex.IsMatch(value))
            throw new ArgumentException("Formato email non valido", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj) => Equals(obj as Email);

    public bool Equals(Email? other) => other is not null && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(Email email) => email.Value;

    public static bool operator ==(Email? left, Email? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(Email? left, Email? right) => !(left == right);
}
