# FidelityCard - Clean Architecture

## 🏗️ Struttura del Progetto

```
FidelityCard/
├── FidelityCard.Domain/          # Layer 1: Domain (Core)
│   ├── Entities/
│   │   └── Fidelity.cs           # Entity senza dipendenze EF
│   ├── ValueObjects/
│   │   ├── Email.cs              # Value Object con validazione
│   │   ├── CodiceNegozio.cs      # Value Object per codice store
│   │   └── CodiceFidelity.cs     # Value Object per codice fidelity
│   └── Interfaces/
│       ├── IFidelityRepository.cs
│       └── ITokenRepository.cs
│
├── FidelityCard.Application/     # Layer 2: Application (Use Cases)
│   ├── UseCases/
│   │   ├── RegisterFidelityUseCase.cs
│   │   ├── ValidateEmailUseCase.cs
│   │   ├── GetProfileUseCase.cs
│   │   ├── ConfirmEmailUseCase.cs
│   │   └── GetFidelityByEmailUseCase.cs
│   ├── DTOs/
│   │   ├── FidelityDto.cs
│   │   ├── RegisterFidelityRequest.cs
│   │   ├── ValidateEmailDto.cs
│   │   ├── GetProfileDto.cs
│   │   └── Result.cs
│   ├── Interfaces/
│   │   ├── IEmailService.cs
│   │   └── ICardGeneratorService.cs
│   └── Mappers/
│       └── FidelityMapper.cs
│
├── FidelityCard.Srv/             # Layer 3: Infrastructure + Presentation
│   ├── Controllers/
│   │   └── FidelityCardController.cs  # Controller sottile
│   ├── Data/
│   │   ├── FidelityCardDbContext.cs
│   │   └── Configurations/
│   │       └── FidelityConfiguration.cs  # Fluent API
│   ├── Repositories/
│   │   ├── FidelityRepository.cs
│   │   └── FileTokenRepository.cs
│   ├── Services/
│   │   ├── EmailService.cs
│   │   └── CardGeneratorService.cs
│   └── Program.cs                # DI Configuration
│
├── FidelityCard.Lib/             # Shared DTOs (per Blazor client)
│   ├── Models/
│   │   ├── FidelityDto.cs
│   │   └── ...
│   └── Services/
│       ├── TokenManager.cs
│       └── EmailSettings.cs
│
├── FidelityCard/                 # Blazor WebAssembly Client (invariato)
│
└── FidelityCard.Tests/           # Unit Tests
    ├── ValueObjects/
    ├── Entities/
    └── UseCases/
```

## 📐 Dependency Flow

```
┌─────────────────┐
│  Presentation   │  Controllers (sottili)
└────────┬────────┘
         │ dipende da
         ▼
┌─────────────────┐
│   Application   │  Use Cases, DTOs, Interfaces
└────────┬────────┘
         │ dipende da
         ▼
┌─────────────────┐
│     Domain      │  Entities, Value Objects, Repository Interfaces
└─────────────────┘
         ▲
         │ implementa
┌────────┴────────┐
│ Infrastructure  │  DbContext, Repositories, Services
└─────────────────┘
```

## 🔑 Principi Applicati

### Domain Layer
- **Entities**: `Fidelity` con factory method e validazione interna
- **Value Objects**: `Email`, `CodiceNegozio`, `CodiceFidelity` con:
  - Validazione nel costruttore
  - Immutabilità
  - Equality basata su valore
  - Conversione implicita a string
- **Repository Interfaces**: Contratti per persistenza

### Application Layer
- **Use Cases**: Single Responsibility, orchestrano la logica
- **DTOs**: Separati dalle entities, usati per I/O
- **Result Pattern**: `Result<T>` per gestione errori senza eccezioni
- **Mapper**: Conversione Entity ↔ DTO

### Infrastructure Layer
- **Fluent API**: Configurazione EF in `FidelityConfiguration.cs`
- **Value Object Converters**: Mapping automatico DB ↔ Value Objects
- **Repository Implementations**: Accesso dati concreto

### Presentation Layer
- **Controller sottile**: Delega tutto ai Use Cases
- **Validazione input**: Solo DataAnnotations, logica nei Use Cases

## 💉 Dependency Injection

```csharp
// Repositories (Scoped)
services.AddScoped<IFidelityRepository, FidelityRepository>();
services.AddScoped<ITokenRepository, FileTokenRepository>();

// Services (Scoped)
services.AddScoped<ICardGeneratorService, CardGeneratorService>();
services.AddScoped<IEmailService, EmailService>();

// Use Cases (Scoped)
services.AddScoped<RegisterFidelityUseCase>();
services.AddScoped<ValidateEmailUseCase>();
services.AddScoped<GetProfileUseCase>();
services.AddScoped<ConfirmEmailUseCase>();
services.AddScoped<GetFidelityByEmailUseCase>();

// Options Pattern
services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
```

## ✅ Validazione

### Value Objects (Domain)
- Email: formato, lunghezza max 100
- CodiceNegozio: 2-6 caratteri alfanumerici
- CodiceFidelity: 6-20 caratteri alfanumerici

### Entity Factory (Domain)
- Data nascita: range -100 / -6 anni
- Cellulare: formato valido
- Campi obbligatori

### DTOs (Application)
- DataAnnotations per validazione API

### Use Cases (Application)
- Business rules: email unica, codice unico

## 🧪 Testing

```bash
dotnet test FidelityCard.Tests
```

### Test Coverage
- Value Objects: validazione, equality, conversioni
- Entities: factory method, ricostituzione
- Use Cases: happy path, edge cases, mock dei repository

## 📝 Migration da Vecchia Architettura

| Vecchio | Nuovo |
|---------|-------|
| `Fidelity.cs` con attributi EF | Entity in Domain + Fluent API |
| `FidelityCardController` con logica | Controller sottile + Use Cases |
| `ITokenService` in Srv | `ITokenRepository` in Domain |
| Servizi iniettati nel controller | Use Cases iniettati |
| `IEmailService` in Lib | `IEmailService` in Application |
