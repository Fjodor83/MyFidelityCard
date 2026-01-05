using FidelityCard.Domain.Interfaces;
using FidelityCard.Lib.Services;

namespace FidelityCard.Srv.Repositories;

/// <summary>
/// Implementazione file-based del repository per Token
/// </summary>
public class FileTokenRepository : ITokenRepository
{
    private readonly IWebHostEnvironment _env;
    private readonly string _tokenPath;

    public FileTokenRepository(IWebHostEnvironment env)
    {
        _env = env;
        _tokenPath = Path.Combine(_env.ContentRootPath, "Token");
        
        if (!Directory.Exists(_tokenPath))
        {
            Directory.CreateDirectory(_tokenPath);
        }
    }

    public string GenerateToken(string email, string store)
    {
        var token = TokenManager.Generate();
        var fileName = Path.Combine(_tokenPath, token);
        
        // Formato: store\r\nemail
        File.WriteAllText(fileName, $"{store}\r\n{email}");
        
        return token;
    }

    public async Task<TokenData?> GetTokenDataAsync(string token, CancellationToken cancellationToken = default)
    {
        // Cleanup opportunistico dei token scaduti
        CleanupExpiredTokens(TimeSpan.FromMinutes(15));

        var fileName = Path.Combine(_tokenPath, token);

        if (!File.Exists(fileName))
        {
            return null;
        }

        var content = await File.ReadAllTextAsync(fileName, cancellationToken);
        var parts = content.Split("\r\n");

        if (parts.Length < 2)
        {
            return null;
        }

        return new TokenData(parts[0], parts[1]);
    }

    public async Task<TokenData?> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await GetTokenDataAsync(token, cancellationToken);
    }

    public void CleanupExpiredTokens(TimeSpan maxAge)
    {
        if (!Directory.Exists(_tokenPath))
        {
            return;
        }

        var files = Directory.EnumerateFiles(_tokenPath);
        var cutoffTime = DateTime.Now.Subtract(maxAge);

        foreach (var file in files)
        {
            try
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.CreationTime < cutoffTime)
                {
                    File.Delete(file);
                }
            }
            catch
            {
                // Ignora errori durante la pulizia
            }
        }
    }
}
