namespace SessionizeInjectionWithScopes.Configuration;

using Microsoft.Maui.LifecycleEvents;
using System.Diagnostics;

internal static class MauiConfiguration
{
	public static MauiAppBuilder AddConfiguration(this MauiAppBuilder builder)
	{
		return builder.UseMauiApp<App>()
					  .AddFonts()
					  .AddHandlers()
					  .AddLifeCycle();
	}

	private static MauiAppBuilder AddFonts(this MauiAppBuilder builder)
	{
		return builder.ConfigureFonts(fonts =>
		{
			fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
		});
	}

	private static MauiAppBuilder AddHandlers(this MauiAppBuilder builder)
	{
		return builder;
#if ANDROID
#else
#endif
	}

	private static MauiAppBuilder AddLifeCycle(this MauiAppBuilder builder)
	{
		return builder.ConfigureLifecycleEvents(events =>
		{
#if ANDROID
			events.AddAndroid(android => android.OnActivityResult((activity, requestCode, resultCode, data) => LogEvent("OnActivityResult", requestCode.ToString()))
												.OnStart((activity) => LogEvent("OnStart"))
												.OnCreate((activity, bundle) => LogEvent("OnCreate"))
												.OnStop((activity) => LogEvent("OnStop")));

#endif
		});
	}

	private static void LogEvent(string eventName, string? type = null)
	{
		Debug.WriteLine($"Lifecycle event: {eventName}{(type == null ? string.Empty : $" ({type})")}");
	}
}
