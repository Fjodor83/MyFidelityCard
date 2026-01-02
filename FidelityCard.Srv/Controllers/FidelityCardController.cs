using FidelityCard.Lib.Models;
using FidelityCard.Lib.Services;
using FidelityCard.Srv.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace FidelityCard.Srv.Controllers;

[ApiController]
[Route("api/[controller]")]
//  [Route("api/Fidelity")]
public class FidelityCardController(FidelityCardDbContext context, 
        ILogger<FidelityCardController> logger,
        IOptions<EmailSettings> emailSettings,
        IConfiguration config,
        IWebHostEnvironment env,
        ICardGeneratorService cardGenerator,
        IEmailService emailService) : ControllerBase
{
    private readonly FidelityCardDbContext _context = context;

    private readonly ILogger<FidelityCardController> _logger = logger;
    private readonly IOptions<EmailSettings> _emailSettings = emailSettings;
    private readonly IConfiguration _config = config;
    private readonly IWebHostEnvironment _env = env;
    private readonly ICardGeneratorService _cardGenerator = cardGenerator;
    private readonly IEmailService _emailService = emailService;

    // GET: api/FidelityCard
    [HttpGet]
    public async Task<Fidelity> Get(string email)
    {
        return await _context.Fidelity.FirstOrDefaultAsync(f => f.Email == email) ?? new Fidelity();
    }


    // GET: api/FidelityCard/EmailValidation
    [HttpGet("[action]")]
    public async Task<IActionResult> EmailValidation(string email, string store)
    {
        // Verifica se l'utente esiste già
        var existingUser = await _context.Fidelity.FirstOrDefaultAsync(f => f.Email == email);

        // Genero token
        var token = TokenManager.Generate();
        System.IO.File.WriteAllText(Path.Combine(_env.ContentRootPath, "Token", token), $"{store}\r\n{email}");

        if (existingUser != null)
        {
            // UTENTE ESISTENTE: Invio link per ACEDERE AL PROFILO
            var url = $"{Request.Scheme}://{_config.GetValue<string>("ClientHost")}/profilo?token={token}";
            await _emailService.InviaEmailAccessoProfiloAsync(email, existingUser.Nome ?? "Cliente", url);
            
            return Ok(new { userExists = true });
        }
        else
        {
            // NUOVO UTENTE: Invio link per COMPLETARE REGISTRAZIONE
            var url = $"{Request.Scheme}://{_config.GetValue<string>("ClientHost")}/Fidelity-form?token={token}";
            
            await _emailService.InviaEmailVerificaAsync(email, "Cliente", token, url, store);
            
            return Ok(new { userExists = false });
        }
    }

    // GET: api/FidelityCard/EmailConfirmation
    [HttpGet("[action]")]
    public async Task<string> EmailConfirmation(string token)
    {
        string pathName = Path.Combine(_env.ContentRootPath, "Token");
        string fileName = Path.Combine(pathName, token);
       
        var files = System.IO.Directory.EnumerateFiles(pathName);

        // cancello i file (relativi ai roken) più vecchi di 15 minuti
        foreach (var file in files) { 
            FileInfo fileInfo = new (file);
            if (fileInfo.CreationTime < DateTime.Now.AddMinutes(-15))
            {
                try
                {
                    System.IO.File.Delete(file);
                }
                catch { }
            }
        }
        
        // leggo il contenuto del file e lo torno indietro
        if (System.IO.File.Exists(fileName)) { 
            string fileContent = await System.IO.File.ReadAllTextAsync(fileName);

            return fileContent;
        }
      
        return string.Empty;
    }

    // GET: api/FidelityCard/Profile
    [HttpGet("Profile")]
    public async Task<IActionResult> GetProfile(string token)
    {
        // Logica validazione token simile a EmailConfirmation
        string pathName = Path.Combine(_env.ContentRootPath, "Token");
        string fileName = Path.Combine(pathName, token);

        if (!System.IO.File.Exists(fileName))
        {
             return NotFound("Token non valido o scaduto");
        }

        string fileContent = await System.IO.File.ReadAllTextAsync(fileName);
        string[] param = fileContent.Split("\r\n");
        string email = param[1];

        var user = await _context.Fidelity.FirstOrDefaultAsync(f => f.Email == email);
        if (user == null)
        {
             return NotFound("Utente non trovato");
        }

        return Ok(user);
    }

    // GET: api/FidelityCard/QRCode/{code}
    [HttpGet("QRCode/{code}")]
    public async Task<IActionResult> GetQrCode(string code)
    {
        if (string.IsNullOrEmpty(code)) return BadRequest();

        // Genera solo il QR Code (immagine)
         using var generator = new QRCoder.QRCodeGenerator();
         var qrCodeData = generator.CreateQrCode(code, QRCoder.QRCodeGenerator.ECCLevel.Q);
         using var qrCode = new QRCoder.PngByteQRCode(qrCodeData);
         var qrCodeBytes = qrCode.GetGraphic(20);

         return File(qrCodeBytes, "image/png");
    }

    // POST: api/FidelityCard
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Fidelity fidelity)
    {

        if (fidelity == null)
        {
            Console.WriteLine("Modello nullo");
            return BadRequest();
        }

        _context.Fidelity.Add(fidelity);
        try
        {
            await _context.SaveChangesAsync();
            
            // Generazione Card e Invio Email
            try
            {
                var cardBytes = await _cardGenerator.GeneraCardDigitaleAsync(fidelity, fidelity.Store);
                await _emailService.InviaEmailBenvenutoAsync(fidelity.Email ?? "", fidelity.Nome ?? "Cliente", fidelity.CdFidelity ?? "", cardBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante generazione card o invio email benvenuto.");
                // Non blocchiamo il ritorno OK, la registrazione è avvenuta
            }

            return Ok(fidelity);
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                return StatusCode(500, ex.InnerException.Message);
            }
            else 
            {
                return StatusCode(500, ex.Message);
            }

        }
    }

}

