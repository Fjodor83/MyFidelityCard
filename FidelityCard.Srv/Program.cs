using FidelityCard.Application.UseCases;
using FidelityCard.Domain.Interfaces;
using FidelityCard.Lib.Services;
using FidelityCard.Srv.Data;
using FidelityCard.Srv.Repositories;
using FidelityCard.Srv.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

// Alias per evitare ambiguità
using IAppEmailService = FidelityCard.Application.Interfaces.IEmailService;
using IAppCardGeneratorService = FidelityCard.Application.Interfaces.ICardGeneratorService;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// CORS Configuration
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7065")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ==========================================
// Options Pattern - Settings
// ==========================================
var emailSettings = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>() ?? new();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton(Options.Create(emailSettings));

// ==========================================
// Database Configuration
// ==========================================
builder.Services.AddDbContext<FidelityCardDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================================
// Repository Registrations (Scoped)
// ==========================================
builder.Services.AddScoped<IFidelityRepository, FidelityRepository>();
builder.Services.AddScoped<ITokenRepository, FileTokenRepository>();

// ==========================================
// Service Registrations (Scoped)
// ==========================================
builder.Services.AddScoped<IAppCardGeneratorService, CardGeneratorService>();
builder.Services.AddScoped<IAppEmailService, EmailService>();

// ==========================================
// Use Case Registrations (Scoped)
// ==========================================
builder.Services.AddScoped<RegisterFidelityUseCase>();
builder.Services.AddScoped<ValidateEmailUseCase>();
builder.Services.AddScoped<GetProfileUseCase>();
builder.Services.AddScoped<ConfirmEmailUseCase>();
builder.Services.AddScoped<GetFidelityByEmailUseCase>();

// ==========================================
// Controllers & Swagger
// ==========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ==========================================
// Middleware Pipeline
// ==========================================
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
