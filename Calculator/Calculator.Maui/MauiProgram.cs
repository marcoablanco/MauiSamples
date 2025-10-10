namespace Calculator.Maui;

using Calculator.Logic.AppConfiguration;
using Calculator.Maui.Features.Calculator;
using Microsoft.Extensions.Logging;

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
		AppBootstrapper.InitAppLogic(builder.Services);
		return builder;
	}

	public static MauiAppBuilder RegisterFeatures(this MauiAppBuilder builder)
	{
		// Main
		builder.Services
			   .AddTransient<AppShell>()

			   // Calculator
			   .AddTransient<CalculatorPage>()
			   .AddTransient<CalculatorViewModel>();

		return builder;
	}
}