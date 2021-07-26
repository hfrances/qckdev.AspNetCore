<a href="https://www.nuget.org/packages/qckdev.AspNetCore"><img src="https://img.shields.io/nuget/v/qckdev.AspNetCore.svg" alt="NuGet Version"/></a>
<a href="https://sonarcloud.io/dashboard?id=qckdev.AspNetCore"><img src="https://sonarcloud.io/api/project_badges/measure?project=qckdev.AspNetCore&metric=alert_status" alt="Quality Gate"/></a>
<a href="https://sonarcloud.io/dashboard?id=qckdev.AspNetCore"><img src="https://sonarcloud.io/api/project_badges/measure?project=qckdev.AspNetCore&metric=coverage" alt="Code Coverage"/></a>
<a><img src="https://hfrances.visualstudio.com/Main/_apis/build/status/qckdev.AspNetCore?branchName=master" alt="Azure Pipelines Status"/></a>

# qckdev.AspNetCore
 
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

	app.UseJsonExceptionHandler;
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

