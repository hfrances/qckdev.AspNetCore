# qckdev.AspNetCore.Identity
 
```cs
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
using Microsoft.Extensions.Configuration;
using qckdev.AspNetCore.Infrastructure.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

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

