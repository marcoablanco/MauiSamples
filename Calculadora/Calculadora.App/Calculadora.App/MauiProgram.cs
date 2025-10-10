using Microsoft.Extensions.Logging;

namespace Calculadora.App;

using Calculadora.App.Services;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseFonts()
			.RegisterServices()
			.RegisterFeatures();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

	public static MauiAppBuilder UseFonts(this MauiAppBuilder builder)
	{
		builder.ConfigureFonts(fonts =>
		{
			fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
		});

		return builder;
	}

	public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
	{
		// Register services here
		builder.Services
			   .AddTransient<IOperationServices>(s=> new OperationServices(s));
		return builder;
	}

	public static MauiAppBuilder RegisterFeatures(this MauiAppBuilder builder)
	{
		// Main
		builder.Services
			   .AddTransient<AppShell>()

			   // Calculator
			   .AddTransient<Features.Calculator.CalculatorPage>()
			   .AddTransient<Features.Calculator.CalculatorViewModel>();

		return builder;
	}
}