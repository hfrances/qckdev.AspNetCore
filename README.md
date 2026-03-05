[![NuGet Version](https://img.shields.io/nuget/v/qckdev.AspNetCore.svg)](https://www.nuget.org/packages/qckdev.AspNetCore)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=qckdev.AspNetCore&metric=alert_status)](https://sonarcloud.io/dashboard?id=qckdev.AspNetCore)
[![Code Coverage](https://sonarcloud.io/api/project_badges/measure?project=qckdev.AspNetCore&metric=coverage)](https://sonarcloud.io/dashboard?id=qckdev.AspNetCore)
![Azure Pipelines Status](https://hfrances.visualstudio.com/Main/_apis/build/status/qckdev.AspNetCore?branchName=master)

# qckdev.AspNetCore

Provides a default set of tools for building an ASP.NET Core application.

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

	app.UseJsonExceptionHandler();
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

## 🤝 Contributing
Issues and pull requests are welcome! See the contribution guidelines (coming soon).

## 📜 License
This project is licensed under the terms of the [MIT License](LICENSE).