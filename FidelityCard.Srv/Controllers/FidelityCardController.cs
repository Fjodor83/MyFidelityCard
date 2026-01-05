using FidelityCard.Application.DTOs;
using FidelityCard.Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace FidelityCard.Srv.Controllers;

/// <summary>
/// Controller sottile che orchestra gli Use Cases
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FidelityCardController : ControllerBase
{
    private readonly RegisterFidelityUseCase _registerFidelityUseCase;
    private readonly ValidateEmailUseCase _validateEmailUseCase;
    private readonly GetProfileUseCase _getProfileUseCase;
    private readonly ConfirmEmailUseCase _confirmEmailUseCase;
    private readonly GetFidelityByEmailUseCase _getFidelityByEmailUseCase;
    private readonly IConfiguration _config;

    public FidelityCardController(
        RegisterFidelityUseCase registerFidelityUseCase,
        ValidateEmailUseCase validateEmailUseCase,
        GetProfileUseCase getProfileUseCase,
        ConfirmEmailUseCase confirmEmailUseCase,
        GetFidelityByEmailUseCase getFidelityByEmailUseCase,
        IConfiguration config)
    {
        _registerFidelityUseCase = registerFidelityUseCase;
        _validateEmailUseCase = validateEmailUseCase;
        _getProfileUseCase = getProfileUseCase;
        _confirmEmailUseCase = confirmEmailUseCase;
        _getFidelityByEmailUseCase = getFidelityByEmailUseCase;
        _config = config;
    }

    /// <summary>
    /// GET: api/FidelityCard - Recupera fidelity per email
    /// </summary>
    [HttpGet]
    public async Task<FidelityDto> Get(string email)
    {
        return await _getFidelityByEmailUseCase.ExecuteAsync(email);
    }

    /// <summary>
    /// GET: api/FidelityCard/EmailValidation - Validazione email e invio link
    /// </summary>
    [HttpGet("[action]")]
    public async Task<IActionResult> EmailValidation(string email, string? store)
    {
        var baseUrl = $"{Request.Scheme}://{_config.GetValue<string>("ClientHost")}";
        
        var result = await _validateEmailUseCase.ExecuteAsync(email, store, baseUrl);

        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(new { userExists = result.Data!.UserExists });
    }

    /// <summary>
    /// GET: api/FidelityCard/EmailConfirmation - Conferma token email
    /// </summary>
    [HttpGet("[action]")]
    public async Task<string> EmailConfirmation(string token)
    {
        return await _confirmEmailUseCase.ExecuteAsync(token);
    }

    /// <summary>
    /// GET: api/FidelityCard/Profile - Recupera profilo tramite token
    /// </summary>
    [HttpGet("Profile")]
    public async Task<IActionResult> GetProfile(string token)
    {
        var result = await _getProfileUseCase.ExecuteAsync(token);

        if (!result.Success)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// GET: api/FidelityCard/QRCode/{code} - Genera QR Code
    /// </summary>
    [HttpGet("QRCode/{code}")]
    public IActionResult GetQrCode(string code)
    {
        if (string.IsNullOrEmpty(code)) 
            return BadRequest();

        using var generator = new QRCodeGenerator();
        var qrCodeData = generator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(20);

        return File(qrCodeBytes, "image/png");
    }

    /// <summary>
    /// POST: api/FidelityCard - Registra nuova fidelity
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RegisterFidelityRequest request)
    {
        if (request == null)
        {
            return BadRequest("Richiesta non valida");
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return BadRequest(new { errors });
        }

        var result = await _registerFidelityUseCase.ExecuteAsync(request);

        if (!result.Success)
        {
            return StatusCode(500, result.ErrorMessage);
        }

        return Ok(result.Data);
    }
}
