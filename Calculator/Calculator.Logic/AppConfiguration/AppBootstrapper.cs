namespace Calculator.Logic.AppConfiguration;

using Calculator.Logic.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public static class AppBootstrapper
{
	public static IServiceCollection InitAppLogic(this IServiceCollection services)
	{
		return services.RegisterServices();
	}

	private static IServiceCollection RegisterServices(this IServiceCollection services)
	{
		return services.AddTransient<IOperationService>(s => new OperationService(s));
	}
}