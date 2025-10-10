namespace Calculator.Logic.AppConfiguration;

using Calculator.Logic.Services;
using Microsoft.Extensions.DependencyInjection;

public static class AppBootstrapper
{
	public static IServiceCollection InitAppLogic(this IServiceCollection services)
	{
		return services.RegisterServices();
	}

	private static IServiceCollection RegisterServices(this IServiceCollection services)
	{
		return services.AddTransient<IOperationServices>(s=> new OperationServices(s));
	}

}