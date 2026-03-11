# qckdev.AspNetCore.FluentValidation

FluentValidation integration for `Microsoft.Extensions.Options`, including startup fail-fast validation compatibility across target frameworks.

## Install

```bash
dotnet add package qckdev.AspNetCore.FluentValidation
```

## Example

```csharp
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using qckdev.AspNetCore.FluentValidation;

services.AddSingleton<IValidator<MySettings>, MySettingsValidator>();

services.AddOptions<MySettings>()
    .Configure(settings)
    .ValidateFluentValidation()
    .ValidateOnStartCompat();
```
