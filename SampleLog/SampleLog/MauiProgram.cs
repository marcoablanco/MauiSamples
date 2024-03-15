namespace SampleLog;

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.ClearProviders()
					   .AddDebug();
#endif
builder.Logging.Services.AddTransient<CustomLogger>();
builder.Logging.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, CustomLoggerProvider>());
builder.Logging.AddFilter<CustomLoggerProvider>(_ => true);


		builder.Services.AddTransient<MainPage>()
						.AddTransient<AppShell>();


		return builder.Build();
	}
}
