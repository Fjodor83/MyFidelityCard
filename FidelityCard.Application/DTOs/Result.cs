namespace FidelityCard.Application.DTOs;

/// <summary>
/// DTO generico per risultati operazioni
/// </summary>
public class Result<T>
{
    public bool Success { get; private set; }
    public T? Data { get; private set; }
    public string? ErrorMessage { get; private set; }
    public IReadOnlyList<string> Errors { get; private set; } = Array.Empty<string>();

    private Result() { }

    public static Result<T> Ok(T data) => new()
    {
        Success = true,
        Data = data
    };

    public static Result<T> Fail(string errorMessage) => new()
    {
        Success = false,
        ErrorMessage = errorMessage
    };

    public static Result<T> Fail(IEnumerable<string> errors) => new()
    {
        Success = false,
        Errors = errors.ToList(),
        ErrorMessage = string.Join("; ", errors)
    };
}

/// <summary>
/// Result senza dati
/// </summary>
public class Result
{
    public bool Success { get; private set; }
    public string? ErrorMessage { get; private set; }
    public IReadOnlyList<string> Errors { get; private set; } = Array.Empty<string>();

    private Result() { }

    public static Result Ok() => new() { Success = true };

    public static Result Fail(string errorMessage) => new()
    {
        Success = false,
        ErrorMessage = errorMessage
    };

    public static Result Fail(IEnumerable<string> errors) => new()
    {
        Success = false,
        Errors = errors.ToList(),
        ErrorMessage = string.Join("; ", errors)
    };
}
