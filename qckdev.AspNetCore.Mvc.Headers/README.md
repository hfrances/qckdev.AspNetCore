# qckdev.AspNetCore.Mvc.Headers

HTTP header metadata, accessor service, and middleware-based validation for ASP.NET Core MVC.

## Install

```bash
dotnet add package qckdev.AspNetCore.Mvc.Headers
```

## Example

```csharp
using Microsoft.AspNetCore.Mvc;
using qckdev.AspNetCore.Http.Metadata;
using qckdev.AspNetCore.Mvc.Headers;

builder.Services.AddHttpHeaderAccessor();

var app = builder.Build();
app.UseHttpHeader<HttpAcceptLanguageHeaderAttribute>();

[ApiController]
[Route("api/localized")]
public sealed class LocalizedController : ControllerBase
{
    [HttpGet]
    [HttpAcceptLanguageHeader(isMandatory: true)]
    public ActionResult<string> Get([FromServices] IHttpHeaderAccessor headers)
    {
        var language = headers.GetHttpHeader<HttpAcceptLanguageHeaderAttribute>();
        return Ok($"Accept-Language: {language}");
    }
}
```
