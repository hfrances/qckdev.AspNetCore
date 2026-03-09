# qckdev.AspNetCore.Mvc.Controllers

Base API controller with MediatR v12 integration, logging, and localized exception handling.

## Install

```bash
dotnet add package qckdev.AspNetCore.Mvc.Controllers
dotnet add package MediatR --version 12.5.0
```

## Example

```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
using qckdev.AspNetCore.Mvc.Controllers;

[Route("api/orders")]
public sealed class OrdersController : ApiControllerBase
{
    [HttpPost]
    public Task Create([FromBody] CreateOrderCommand command, CancellationToken ct)
        => Send(command, ct);
}

public sealed record CreateOrderCommand(string CustomerId) : IRequest;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand>
{
    public Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
```

## MediatR Registration

```csharp
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommandHandler).Assembly));
```
