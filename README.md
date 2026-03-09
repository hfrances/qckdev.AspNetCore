[![NuGet Version](https://img.shields.io/nuget/v/qckdev.AspNetCore.svg)](https://www.nuget.org/packages/qckdev.AspNetCore)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=qckdev.AspNetCore&metric=alert_status)](https://sonarcloud.io/dashboard?id=qckdev.AspNetCore)
[![Code Coverage](https://sonarcloud.io/api/project_badges/measure?project=qckdev.AspNetCore&metric=coverage)](https://sonarcloud.io/dashboard?id=qckdev.AspNetCore)
![Azure Pipelines Status](https://hfrances.visualstudio.com/Main/_apis/build/status/qckdev.AspNetCore?branchName=master)

# qckdev.AspNetCore

Toolkit for building ASP.NET Core applications with reusable middleware, MVC helpers, headers validation, and abstractions.

## Packages

This repository contains the following packable libraries:

- `qckdev.AspNetCore`:
  [Package README](./qckdev.AspNetCore/README.md)
- `qckdev.AspNetCore.Abstractions`:
  [Package README](./qckdev.AspNetCore.Abstractions/README.md)
- `qckdev.AspNetCore.Mvc.Controllers` (MediatR v12):
  [Package README](./qckdev.AspNetCore.Mvc.Controllers/README.md)
- `qckdev.AspNetCore.Mvc.Controllers.Legacy` (MediatR v11):
  [Package README](./qckdev.AspNetCore.Mvc.Controllers.Legacy/README.md)
- `qckdev.AspNetCore.Mvc.Headers`:
  [Package README](./qckdev.AspNetCore.Mvc.Headers/README.md)

## Quick Start

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using qckdev.AspNetCore.Localization;
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
```

## 🤝 Contributing
Issues and pull requests are welcome! See the contribution guidelines (coming soon).

## 📜 License
This project is licensed under the terms of the [MIT License](LICENSE).