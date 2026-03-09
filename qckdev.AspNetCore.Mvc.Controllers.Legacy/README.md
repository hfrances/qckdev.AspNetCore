# qckdev.AspNetCore.Mvc.Controllers.Legacy

Base API controller for legacy targets with MediatR v11 integration, logging, and localized exception handling.

## Install

```bash
dotnet add package qckdev.AspNetCore.Mvc.Controllers.Legacy
dotnet add package MediatR.Extensions.Microsoft.DependencyInjection --version 11.0.0
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
    public Task<Unit> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Unit.Value);
    }
}
```

## MediatR Registration

```csharp
builder.Services.AddMediatR(typeof(CreateOrderCommandHandler).Assembly);
```
