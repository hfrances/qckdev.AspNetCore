# qckdev.AspNetCore.Identity
 
```cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using qckdev.AspNetCore;
using qckdev.AspNetCore.Middleware;

public void ConfigureServices(IServiceCollection services)
{
	services.AddDataInitializer<DataInitialization>();
	services.AddControllers();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
	(...)

	app.UseMiddleware<HandlerExceptionMiddleware>();
	app.UseRouting();

	(...)

	app.DataInitialization();
}
```

```cs
using MediatR;
using Microsoft.Extensions.Configuration;
using qckdev.AspNetCore.Identity.Commands;
using qckdev.AspNetCore.Identity.Infrastructure.Data;
using qckdev.AspNetCore.Identity.Services;

public class DataInitialization : IDataInitializer
{
	public DataInitialization(
			IServiceProvider services,
			IConfiguration configuration, 
			...)
	{
		(...)
	}

	public async Task InitializeAsync(CancellationToken cancellationToken)
	{
		(...)
	}
}
```

