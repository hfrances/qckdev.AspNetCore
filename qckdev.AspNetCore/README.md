# qckdev.AspNetCore

Provides base infrastructure helpers for ASP.NET Core applications:
- Host environment service registration
- JSON serialized exception middleware
- Data initialization pipeline
- Localization bootstrap
- Swagger bootstrap helpers

## Install

```bash
dotnet add package qckdev.AspNetCore
```

## Example

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using qckdev.AspNetCore.Localization;
using qckdev.AspNetCore.Persistence;
using qckdev.AspNetCore.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHostEnvironmentService();
builder.Services.AddDataInitializer<SeedDataInitializer>();
builder.Services.AddLocalization<ApplicationResource>("en-US");
builder.Services.AddSwagger(c => c.AddSecurityBearer("Bearer"));

var app = builder.Build();

app.UseSerializedExceptionHandler();
app.UseRouting();
app.UseLocalization();
app.UseSwagger();
app.UseDataInitializer();
app.MapControllers();
app.Run();

public sealed class ApplicationResource : IApplicationResource { }

public sealed class SeedDataInitializer : IDataInitializer
{
    public Task InitializeAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
```
